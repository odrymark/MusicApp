namespace Api.DTOs.Response;

public class SongResDto
{
    public Guid id { get; set; }

    public string title { get; set; }

    public string songKey { get; set; }
    
    public string artist { get; set; }

    public string? image { get; set; }
}