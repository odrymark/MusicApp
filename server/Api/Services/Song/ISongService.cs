using Api.DTOs.Response;

namespace Api.Services.Song;

public interface ISongService
{
    Task CreateSong(Guid userId, string title, string songKey, string artist, bool isPublic, string? imageKey = null);
    Task<IEnumerable<SongResDto>> GetUserSongsAsync(Guid userId);
    Task<IEnumerable<SongResDto>> GetSongs();
}