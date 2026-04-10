using Api.Controllers;
using Api.DTOs.Request;
using Api.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Test.ControllerTests.UserTests;

public class UserControllerTests
{
    private readonly IUserService _mockUserService;
    private readonly UserController _controller;
    private readonly UserControllerStartup _startup;

    public UserControllerTests()
    {
        var services = new ServiceCollection();
        _startup = new UserControllerStartup();
        _startup.ConfigureServices(services);

        var provider = services.BuildServiceProvider();
        _mockUserService = provider.GetRequiredService<IUserService>();
        _controller = _startup.GetController(provider);
    }

    [Fact]
    public async Task CreateUser_Returns_Ok_On_Success()
    {
        var dto = new UserCreateReqDto 
        { 
            username = "newuser", 
            password = "securePassword123" 
        };

        var result = await _controller.CreateUser(dto);

        Assert.IsType<OkResult>(result);
        await _mockUserService.Received(1).CreateUser(dto);
    }

    [Fact]
    public async Task CreateUser_Returns_BadRequest_On_Exception()
    {
        var dto = new UserCreateReqDto { username = "existinguser" };
        var errorMessage = "Username already taken";
        
        _mockUserService.CreateUser(dto).Throws(new Exception(errorMessage));

        var result = await _controller.CreateUser(dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
}