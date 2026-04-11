using Api.Controllers;
using Api.Services.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Test.ControllerTests.UserTests;

public class UserControllerStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Substitute.For<IUserService>());
        services.AddTransient<UserController>();
    }
}