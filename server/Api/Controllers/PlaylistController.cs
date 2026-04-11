using System.Security.Claims;
using Api.DTOs.Request;
using Api.DTOs.Response;
using Api.Services.Playlist;
using Api.Services.R2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/playlist")]
public class PlaylistController(IPlaylistService playlistService, IR2Service r2Service) : ControllerBase
{
    [HttpGet("getPlaylists")]
    public async Task<IActionResult> GetPlaylists()
    {
        try
        {
            var playlists = await playlistService.GetPlaylists();
            return Ok(playlists);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("getUserPlaylists")]
    [Authorize]
    public async Task<IActionResult> GetUserPlaylists()
    {
        try
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = Guid.Parse(idStr!);
            
            var playlists = await playlistService.GetUserPlaylists(id);
            return Ok(playlists);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("createPlaylist")]
    [Authorize]
    public async Task<IActionResult> CreatePlaylist([FromForm] PlaylistCreateReqDto dto)
    {
        try
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = Guid.Parse(idStr!);

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

            await playlistService.CreatePlaylist(id, dto.title, dto.songIds, dto.isPublic, imgKey);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("editPlaylist")]
    [Authorize]
    public async Task<IActionResult> EditPlaylist([FromForm] PlaylistEditReqDto dto)
    {
        try
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = Guid.Parse(idStr!);

            string? imgKey = null;
            if (dto.image != null)
            {
                Console.WriteLine($"Image received: {dto.image.FileName}, size: {dto.image.Length}");
                imgKey = await r2Service.UploadImageStorage(dto.image);
                Console.WriteLine($"Image uploaded with key: {imgKey}");
                
                await r2Service.DeleteFile(dto.prevImgKey!);
            }
            else
            {
                Console.WriteLine("No image provided");
            }
            
            await playlistService.EditPlaylist(id, dto.id, dto.title, dto.songIds, dto.isPublic, imgKey);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}