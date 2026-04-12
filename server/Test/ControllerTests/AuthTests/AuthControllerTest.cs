using Api.Controllers;
using Api.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit.DependencyInjection;

namespace Test.ControllerTests.AuthTests;

[Startup(typeof(AuthControllerStartup))]
public class AuthControllerTests(IAuthService mockAuthService, IServiceProvider provider)
{
    private readonly AuthController _controller = AuthControllerStartup.GetController(provider);

    [Fact]
    public async Task Refresh_Returns_Ok_With_Valid_RefreshToken()
    {
        var refreshToken = "valid-rt";
        AuthControllerStartup.SetupRequestCookies(_controller, new() { ["refreshToken"] = refreshToken });
        
        mockAuthService.RefreshToken(refreshToken, 7).Returns(("new-jwt", "new-rt"));

        var result = await _controller.Refresh();

        Assert.IsType<OkResult>(result);
        await mockAuthService.Received(1).RefreshToken(refreshToken, 7);
    }

    [Fact]
    public async Task Refresh_Returns_Unauthorized_When_No_RefreshToken()
    {
        AuthControllerStartup.SetupRequestCookies(_controller, new Dictionary<string, string>());
    
        var result = await _controller.Refresh();

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    
        var errorObject = unauthorizedResult.Value;
        var messageProperty = errorObject?.GetType().GetProperty("message");
        var message = messageProperty?.GetValue(errorObject)?.ToString();
    
        Assert.Equal("Missing refresh token", message);
    }

    [Fact]
    public async Task Logout_Clears_Cookies_Properly()
    {
        AuthControllerStartup.SetupRequestCookies(_controller, new() { ["refreshToken"] = "active-token" });

        var result = await _controller.Logout();

        Assert.IsType<OkResult>(result);
        var setCookieHeader = _controller.Response.Headers.SetCookie;
        Assert.True(setCookieHeader.Count > 0, "The Set-Cookie header should not be empty.");
    }
}