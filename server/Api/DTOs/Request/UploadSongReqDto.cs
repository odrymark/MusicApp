using Microsoft.AspNetCore.Mvc;

namespace Api.DTOs.Request;

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class UploadSongReqDto
{
    [Required]
    public IFormFile File { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
}