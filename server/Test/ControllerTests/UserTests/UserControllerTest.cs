using Api.Controllers;
using Api.DTOs.Request;
using Api.Services.User;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit.DependencyInjection;

namespace Test.ControllerTests.UserTests;

[Startup(typeof(UserControllerStartup))]
public class UserControllerTests(IUserService mockUserService, UserController controller)
{
    [Fact]
    public async Task CreateUser_Returns_Ok_On_Success()
    {
        var dto = new UserCreateReqDto 
        { 
            username = "newuser",
            password = "securePassword123",
            email = "test123456789@email.com",
            passwordConfirm = "securePassword123"
        };

        var result = await controller.CreateUser(dto);

        Assert.IsType<OkResult>(result);
        await mockUserService.Received(1).CreateUser(dto);
    }

    [Fact]
    public async Task CreateUser_Returns_BadRequest_On_Exception()
    {
        var dto = new UserCreateReqDto { username = "existinguser", password = "securePassword123", passwordConfirm = "securePassword123", email = "existinguser@email.com"};
        var errorMessage = "Username already taken";
        
        mockUserService.CreateUser(dto).Throws(new Exception(errorMessage));

        var result = await controller.CreateUser(dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
}