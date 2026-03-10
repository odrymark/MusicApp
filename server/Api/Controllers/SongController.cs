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

            var songKey = await r2Service.UploadSongStorage(dto.file);
            await songService.CreateSong(id, dto.title, songKey, dto.artist, dto.isPublic);

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

    [HttpGet("getSongs")]
    public async Task<IActionResult> GetSongs()
    {
        try
        {
            var songs = await songService.GetSongs();
            return Ok(songs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("getSongUrl")]
    public IActionResult GetSongUrl(string key)
    {
        try
        {
            var url = r2Service.GenerateSignedUrl(key);
            return Ok(url);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}