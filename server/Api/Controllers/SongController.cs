using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/song")]
public class SongController(R2Service r2Service) : ControllerBase
{
    [HttpPost("uploadSong")]
    public async Task<IActionResult> Upload([FromForm(Name = "file")] IFormFile file)
    {
        Console.WriteLine($"Uploading file {file.FileName} {file.Length} bytes");
        
        try
        {
            var key = await r2Service.UploadFileAsync(file);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}