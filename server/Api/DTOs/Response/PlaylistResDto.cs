namespace Api.DTOs.Response;

public class PlaylistResDto
{
    public Guid id { get; set; }

    public string title { get; set; }
    
    public List<SongResDto> songs { get; set; }
    
    public string creatorUser { get; set; }

    public string? image { get; set; }
    
    public bool isPublic { get; set; }
}