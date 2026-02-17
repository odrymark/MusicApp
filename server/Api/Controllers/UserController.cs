using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(UserService service) : ControllerBase
{
    [HttpPost("createUser")]
    public async Task<ActionResult> CreateUser([FromBody] UserCreateReqDto userCreateReqDto)
    {
        try
        {
            await service.CreateUser(userCreateReqDto);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}