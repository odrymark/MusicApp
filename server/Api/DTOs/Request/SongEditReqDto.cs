using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class SongEditReqDto
{
    [Required]
    public Guid id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string artist { get; set; } = string.Empty;
    
    [Required]
    public bool isPublic { get; set; }
    
    public IFormFile? image { get; set; }
    public string prevImgKey { get; set; }
}