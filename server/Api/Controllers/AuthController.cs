using System.Security.Claims;
using Api.DTOs.Request;
using Api.DTOs.Response;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;
    private readonly IConfiguration _configuration;

    private readonly double JwtExpireMinutes;
    private readonly double RefreshExpireDays;

    public AuthController(AuthService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;

        var jwtSection = _configuration.GetSection("Jwt");
        JwtExpireMinutes = double.Parse(jwtSection["ExpireMinutes"]!);

        var refreshSection = _configuration.GetSection("RefreshToken");
        RefreshExpireDays = double.Parse(refreshSection["ExpireDays"]!);
    }

    private void SetJwtCookie(string token)
    {
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,         //CHANGE FOR PRODUCTION
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(JwtExpireMinutes)
        });
    }

    private void SetRefreshCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,          //CHANGE FOR PRODUCTION
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(RefreshExpireDays)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserLoginReqDto loginReqDto)
    {
        var output = await _service.Login(loginReqDto);

        SetJwtCookie(output.token);
        SetRefreshCookie(output.refreshToken);

        return Ok(new GetMeResDto
        {
            id = output.id,
            username = output.username,
            isAdmin = output.isAdmin
        });
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<ActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _service.Logout(refreshToken);
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,         //CHANGE FOR PRODUCTION
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(-1)
        };

        Response.Cookies.Delete("jwt", cookieOptions);
        Response.Cookies.Delete("refreshToken", cookieOptions);

        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult GetMe()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return Ok(new GetMeResDto
        {
            id = userId,
            username = User.Identity?.Name!,
            isAdmin = User.IsInRole("Admin")
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("Missing refresh token");

        var (token, refresh) = await _service.RefreshToken(refreshToken, RefreshExpireDays);

        SetJwtCookie(token);
        SetRefreshCookie(refresh);

        return Ok();
    }
}
