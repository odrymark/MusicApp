using Api.Controllers;
using Api.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit.DependencyInjection;

namespace Test.ControllerTests.AuthTests;

[Startup(typeof(AuthControllerStartup))]
public class AuthControllerTests
{
    private readonly IAuthService _mockAuthService;
    private readonly AuthController _controller;
    private readonly AuthControllerStartup _startup;
    private readonly IServiceProvider _provider;

    public AuthControllerTests(IAuthService mockAuthService, IServiceProvider provider, AuthControllerStartup startup)
    {
        _mockAuthService = mockAuthService;
        _startup = startup;
        _provider = provider;
        _controller = startup.GetController(provider);
    }

    [Fact]
    public async Task Refresh_Returns_Ok_With_Valid_RefreshToken()
    {
        var refreshToken = "valid-rt";
        _startup.SetupRequestCookies(_controller, new() { ["refreshToken"] = refreshToken });
        
        _mockAuthService.RefreshToken(refreshToken, 7).Returns(("new-jwt", "new-rt"));

        var result = await _controller.Refresh();

        Assert.IsType<OkResult>(result);
        await _mockAuthService.Received(1).RefreshToken(refreshToken, 7);
    }

    [Fact]
    public async Task Refresh_Returns_Unauthorized_When_No_RefreshToken()
    {
        _startup.SetupRequestCookies(_controller, new Dictionary<string, string>());
        
        var result = await _controller.Refresh();

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Missing refresh token", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Logout_Clears_Cookies_Properly()
    {
        _startup.SetupRequestCookies(_controller, new() { ["refreshToken"] = "active-token" });

        var result = await _controller.Logout();

        Assert.IsType<OkResult>(result);
        var setCookieHeader = _controller.Response.Headers.SetCookie;
        Assert.True(setCookieHeader.Count > 0, "The Set-Cookie header should not be empty.");
    }
}