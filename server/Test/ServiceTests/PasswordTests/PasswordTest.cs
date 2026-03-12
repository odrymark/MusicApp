using Api.Services.Password;
using DataAccess;
using Xunit.DependencyInjection;

namespace Test.ServiceTests.PasswordTests;

[Startup(typeof(PasswordStartup))]
public class PasswordServiceTests : TestBase
{
    private readonly IPasswordService _passwordService;

    public PasswordServiceTests(MusicDbContext db, IPasswordService passwordService)
        : base(db)
    {
        _passwordService = passwordService;
    }

    // -------------------------
    // HashPassword Tests
    // -------------------------

    [Fact]
    public void HashPassword_Returns_NonEmpty_Hash()
    {
        var hash = _passwordService.HashPassword("mypassword");

        Assert.False(string.IsNullOrWhiteSpace(hash));
    }

    [Fact]
    public void HashPassword_Same_Input_Produces_Different_Hashes()
    {
        var hash1 = _passwordService.HashPassword("mypassword");
        var hash2 = _passwordService.HashPassword("mypassword");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_Different_Inputs_Produce_Different_Hashes()
    {
        var hash1 = _passwordService.HashPassword("password1");
        var hash2 = _passwordService.HashPassword("password2");

        Assert.NotEqual(hash1, hash2);
    }

    // -------------------------
    // VerifyHashedPassword Tests
    // -------------------------

    [Fact]
    public void VerifyHashedPassword_Returns_True_When_Correct()
    {
        var hash = _passwordService.HashPassword("correctpassword");

        var result = _passwordService.VerifyHashedPassword(hash, "correctpassword");

        Assert.True(result);
    }

    [Fact]
    public void VerifyHashedPassword_Returns_False_When_Wrong()
    {
        var hash = _passwordService.HashPassword("correctpassword");

        var result = _passwordService.VerifyHashedPassword(hash, "wrongpassword");

        Assert.False(result);
    }

    [Fact]
    public void VerifyHashedPassword_Returns_False_When_Hash_Invalid()
    {
        var result = _passwordService.VerifyHashedPassword("not_a_valid_hash", "password");

        Assert.False(result);
    }

    [Fact]
    public void VerifyHashedPassword_Returns_False_When_Empty_Hash()
    {
        var result = _passwordService.VerifyHashedPassword(string.Empty, "password");

        Assert.False(result);
    }

    // -------------------------
    // HashRefreshToken Tests
    // -------------------------

    [Fact]
    public void HashRefreshToken_Returns_NonEmpty_Hash()
    {
        var hash = _passwordService.HashRefreshToken("mytoken");

        Assert.False(string.IsNullOrWhiteSpace(hash));
    }

    [Fact]
    public void HashRefreshToken_Same_Input_Produces_Same_Hash()
    {
        var hash1 = _passwordService.HashRefreshToken("mytoken");
        var hash2 = _passwordService.HashRefreshToken("mytoken");

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashRefreshToken_Different_Inputs_Produce_Different_Hashes()
    {
        var hash1 = _passwordService.HashRefreshToken("token1");
        var hash2 = _passwordService.HashRefreshToken("token2");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashRefreshToken_Returns_Uppercase_Hex()
    {
        var hash = _passwordService.HashRefreshToken("mytoken");

        Assert.Matches("^[0-9A-F]+$", hash);
    }

    [Fact]
    public void HashRefreshToken_Returns_64_Char_Sha256_Hash()
    {
        var hash = _passwordService.HashRefreshToken("mytoken");

        Assert.Equal(64, hash.Length);
    }
}