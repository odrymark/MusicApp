using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request;

public class PlaylistEditReqDto
{
    [Required]
    public Guid id { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string title { get; set; }
    
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public List<Guid> songIds { get; set; }
    
    [Required]
    public bool isPublic { get; set; }
    
    public IFormFile? image { get; set; }
    public string prevImgKey { get; set; }
}