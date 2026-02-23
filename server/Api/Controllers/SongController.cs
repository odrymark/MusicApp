using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/song")]
public class SongController(R2Service r2Service, SongService songService) : ControllerBase
{
    [Authorize]
    [HttpPost("uploadSong")]
    public async Task<IActionResult> UploadSong([FromForm] UploadSongReqDto dto)
    {
        try
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = Guid.Parse(idStr!);

            var songKey = await r2Service.UploadSongStorage(dto.File);
            await songService.CreateSong(id, dto.Title, songKey);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("getUserSongs")]
    public async Task<IActionResult> GetUserSongs()
    {
        try
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = Guid.Parse(idStr!);
            
            var songs = await songService.GetUserSongsAsync(id);
            
            return Ok(songs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}