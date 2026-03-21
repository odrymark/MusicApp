using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.DTOs.Request;
using Api.Services;
using Api.Services.R2;
using Api.Services.Song;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/song")]
public class SongController(IR2Service r2Service, ISongService songService) : ControllerBase
{
    [Authorize]
    [HttpPost("uploadSong")]
    public async Task<IActionResult> UploadSong([FromForm] UploadSongReqDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = Guid.Parse(idStr!);

            var songKey = await r2Service.UploadSongStorage(dto.file);
        
            string? imgKey = null;
            if (dto.image != null)
            {
                Console.WriteLine($"Image received: {dto.image.FileName}, size: {dto.image.Length}");
                imgKey = await r2Service.UploadImageStorage(dto.image);
                Console.WriteLine($"Image uploaded with key: {imgKey}");
            }
            else
            {
                Console.WriteLine("No image provided");
            }

            await songService.CreateSong(id, dto.title, songKey, dto.artist, dto.isPublic, imgKey);

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
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

    [HttpGet("getSignedUrl")]
    public IActionResult GetSignedUrl(string key)
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