using Api.DTOs.Request;
using Api.Services.Auth;
using Api.Services.Password;
using Api.Services.Token;
using DataAccess;
using Xunit.DependencyInjection;

namespace Test.ServiceTests.AuthTests;

[Startup(typeof(AuthStartup))]
public class AuthServiceTests : TestBase
{
    private readonly IAuthService _authService;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public AuthServiceTests(MusicDbContext db, IAuthService authService,
        IPasswordService passwordService, ITokenService tokenService)
        : base(db)
    {
        _authService = authService;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

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
        Assert.Equal("jwt_token", result.token);
    }

    [Fact]
    public async Task Login_Stores_Hashed_RefreshToken_In_Db()
    {
        var user = await CreateUserAsync("login_hash_" + Guid.NewGuid().ToString("N"), password: "hashed_pw");

        await _authService.Login(new UserLoginReqDto { username = user.username, password = "plaintext" });

        await Db.Entry(user).ReloadAsync(Ct); 
        Assert.Equal("HASH_refresh_token_new", user.refreshToken);
    }

    [Fact]
    public async Task Login_Sets_RefreshToken_Expiry()
    {
        var user = await CreateUserAsync("login_expiry_" + Guid.NewGuid().ToString("N"), password: "hashed_pw");
        var before = DateTime.UtcNow.AddDays(6);

        await _authService.Login(new UserLoginReqDto { username = user.username, password = "plaintext" });

        await Db.Entry(user).ReloadAsync(Ct);
        Assert.True(user.refreshTokenExpiry > before);
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

        await Db.Entry(user).ReloadAsync(Ct);
        Assert.Equal("HASH_refresh_token_new", user.refreshToken);
    }

    [Fact]
    public async Task Logout_Clears_RefreshToken_And_Expiry()
    {
        var plainRefresh = "refresh_" + Guid.NewGuid().ToString("N");
        var user = await CreateUserAsync(
            "logout_valid_" + Guid.NewGuid().ToString("N"),
            refreshToken: "HASH_" + plainRefresh,
            refreshTokenExpiry: DateTime.UtcNow.AddDays(5));

        await _authService.Logout(plainRefresh);

        await Db.Entry(user).ReloadAsync(Ct);
        Assert.Null(user.refreshToken);
        Assert.Null(user.refreshTokenExpiry);
    }
}