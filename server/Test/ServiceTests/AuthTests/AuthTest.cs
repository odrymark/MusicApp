using Api.DTOs.Request;
using Api.Services.Auth;
using Api.Services.Password;
using Api.Services.Token;
using DataAccess;
using NSubstitute;
using Xunit;
using Xunit.DependencyInjection;

namespace Test.ServiceTests.AuthTests;

[Startup(typeof(AuthStartup))]
public class AuthServiceTests : TestBase
{
    private readonly IAuthService _authService;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthServiceTests(MusicDbContext db, IAuthService authService,
        IPasswordService passwordService, ITokenService tokenService)
        : base(db)
    {
        _authService = authService;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    // -------------------------
    // Login Tests
    // -------------------------

    [Fact]
    public async Task Login_Returns_Dto_When_Valid()
    {
        var user = await CreateUserAsync("login_valid_" + Guid.NewGuid().ToString("N"), password: "hashed_pw");

        var result = await _authService.Login(new UserLoginReqDto
        {
            username = user.username,
            password = "plaintext"
        });

        Assert.Equal(user.id, result.id);
        Assert.Equal(user.username, result.username);
        Assert.Equal(user.isAdmin, result.isAdmin);
        Assert.Equal("jwt_token", result.token);
        Assert.Equal("refresh_token_new", result.refreshToken);
    }

    [Fact]
    public async Task Login_Stores_Hashed_RefreshToken_In_Db()
    {
        var user = await CreateUserAsync("login_hash_" + Guid.NewGuid().ToString("N"), password: "hashed_pw");

        await _authService.Login(new UserLoginReqDto { username = user.username, password = "plaintext" });

        await Db.Entry(user).ReloadAsync();
        Assert.Equal("HASH_refresh_token_new", user.refreshToken);
    }

    [Fact]
    public async Task Login_Sets_RefreshToken_Expiry()
    {
        var user = await CreateUserAsync("login_expiry_" + Guid.NewGuid().ToString("N"), password: "hashed_pw");
        var before = DateTime.UtcNow.AddDays(6);

        await _authService.Login(new UserLoginReqDto { username = user.username, password = "plaintext" });

        await Db.Entry(user).ReloadAsync();
        Assert.True(user.refreshTokenExpiry > before);
    }

    [Fact]
    public async Task Login_Throws_When_User_Not_Found()
    {
        await Assert.ThrowsAsync<Exception>(() => _authService.Login(new UserLoginReqDto
        {
            username = "ghost_" + Guid.NewGuid().ToString("N"),
            password = "password"
        }));
    }

    [Fact]
    public async Task Login_Throws_When_Password_Invalid()
    {
        var user = await CreateUserAsync("login_badpw_" + Guid.NewGuid().ToString("N"), password: "hashed_pw");
        _passwordService.VerifyHashedPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        await Assert.ThrowsAsync<Exception>(() => _authService.Login(new UserLoginReqDto
        {
            username = user.username,
            password = "wrong"
        }));

        _passwordService.VerifyHashedPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
    }

    // -------------------------
    // RefreshToken Tests
    // -------------------------

    [Fact]
    public async Task RefreshToken_Returns_NewTokens_When_Valid()
    {
        var plainRefresh = "refresh_" + Guid.NewGuid().ToString("N");
        var user = await CreateUserAsync(
            "refresh_valid_" + Guid.NewGuid().ToString("N"),
            refreshToken: "HASH_" + plainRefresh,
            refreshTokenExpiry: DateTime.UtcNow.AddDays(1));

        var (token, refresh) = await _authService.RefreshToken(plainRefresh, 7);

        Assert.Equal("jwt_token", token);
        Assert.Equal("refresh_token_new", refresh);
    }

    [Fact]
    public async Task RefreshToken_Updates_Hashed_Token_In_Db()
    {
        var plainRefresh = "refresh_" + Guid.NewGuid().ToString("N");
        var user = await CreateUserAsync(
            "refresh_db_" + Guid.NewGuid().ToString("N"),
            refreshToken: "HASH_" + plainRefresh,
            refreshTokenExpiry: DateTime.UtcNow.AddDays(1));

        await _authService.RefreshToken(plainRefresh, 7);

        await Db.Entry(user).ReloadAsync();
        Assert.Equal("HASH_refresh_token_new", user.refreshToken);
    }

    [Fact]
    public async Task RefreshToken_Updates_Expiry()
    {
        var plainRefresh = "refresh_" + Guid.NewGuid().ToString("N");
        var user = await CreateUserAsync(
            "refresh_expiry_" + Guid.NewGuid().ToString("N"),
            refreshToken: "HASH_" + plainRefresh,
            refreshTokenExpiry: DateTime.UtcNow.AddDays(1));

        await _authService.RefreshToken(plainRefresh, 14);

        await Db.Entry(user).ReloadAsync();
        Assert.True(user.refreshTokenExpiry > DateTime.UtcNow.AddDays(13));
    }

    [Fact]
    public async Task RefreshToken_Throws_When_Token_Not_Found()
    {
        await Assert.ThrowsAsync<Exception>(() =>
            _authService.RefreshToken("nonexistent_" + Guid.NewGuid().ToString("N"), 7));
    }

    [Fact]
    public async Task RefreshToken_Throws_When_Token_Expired()
    {
        var plainRefresh = "refresh_" + Guid.NewGuid().ToString("N");
        await CreateUserAsync(
            "refresh_expired_" + Guid.NewGuid().ToString("N"),
            refreshToken: "HASH_" + plainRefresh,
            refreshTokenExpiry: DateTime.UtcNow.AddDays(-1));

        await Assert.ThrowsAsync<Exception>(() =>
            _authService.RefreshToken(plainRefresh, 7));
    }

    // -------------------------
    // Logout Tests
    // -------------------------

    [Fact]
    public async Task Logout_Clears_RefreshToken_And_Expiry()
    {
        var plainRefresh = "refresh_" + Guid.NewGuid().ToString("N");
        var user = await CreateUserAsync(
            "logout_valid_" + Guid.NewGuid().ToString("N"),
            refreshToken: "HASH_" + plainRefresh,
            refreshTokenExpiry: DateTime.UtcNow.AddDays(5));

        await _authService.Logout(plainRefresh);

        await Db.Entry(user).ReloadAsync();
        Assert.Null(user.refreshToken);
        Assert.Null(user.refreshTokenExpiry);
    }

    [Fact]
    public async Task Logout_Throws_When_Token_Not_Found()
    {
        await Assert.ThrowsAsync<Exception>(() =>
            _authService.Logout("invalid_" + Guid.NewGuid().ToString("N")));
    }
}