namespace Api.DTOs.Response;

public class SongResDto
{
    public Guid id { get; set; }
    
    public string title { get; set; } = string.Empty;
    
    public string songKey { get; set; } = string.Empty;

    public string? image { get; set; }
}