using Api.Controllers;
using Api.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Test.ControllerTests.AuthTests;

public class AuthControllerStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var config = Substitute.For<IConfiguration>();
        
        var jwtSection = Substitute.For<IConfigurationSection>();
        jwtSection["ExpireMinutes"].Returns("15");
        config.GetSection("Jwt").Returns(jwtSection);
        
        var refreshSection = Substitute.For<IConfigurationSection>();
        refreshSection["ExpireDays"].Returns("7");
        config.GetSection("RefreshToken").Returns(refreshSection);

        services.AddSingleton(config);
        services.AddSingleton(Substitute.For<IAuthService>());
        services.AddTransient<AuthController>();
        services.AddSingleton(this);
    }

    public AuthController GetController(IServiceProvider provider)
    {
        var controller = provider.GetRequiredService<AuthController>();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

   
    public void SetupRequestCookies(AuthController controller, Dictionary<string, string> cookies)
    {
        var mockCookies = Substitute.For<IRequestCookieCollection>();

        foreach (var cookie in cookies)
        {
            mockCookies[cookie.Key].Returns(cookie.Value);

            mockCookies.TryGetValue(cookie.Key, out Arg.Any<string?>())
                .Returns(x => 
                {
                    x[1] = cookie.Value; 
                    return true;
                });
        }

        controller.ControllerContext.HttpContext.Request.Cookies = mockCookies;
    }
}