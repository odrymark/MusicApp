using Microsoft.AspNetCore.Mvc;

namespace Api.DTOs.Request;

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class UploadSongReqDto
{
    [Required]
    public IFormFile file { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string artist { get; set; } = string.Empty;
    
    [Required]
    public bool isPublic { get; set; }
    
    public IFormFile? image { get; set; }
}