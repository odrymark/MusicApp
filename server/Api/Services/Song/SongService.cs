using Api.DTOs.Response;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Song;

public class SongService(MusicDbContext context) : ISongService
{
    public async Task CreateSong(Guid userId, string title, string songKey,  string artist, bool isPublic, string? imageKey = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(songKey))
            throw new ArgumentException("songKey cannot be empty", nameof(songKey));

        var song = new DataAccess.Song
        {
            id = Guid.NewGuid(),
            userId = userId,
            title = title,
            songKey = songKey,
            artist = artist,
            isPublic = isPublic,
            image = imageKey
        };

        context.Songs.Add(song);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SongResDto>> GetUserSongsAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid user ID", nameof(userId));

        var songs = await context.Songs
            .Where(s => s.userId == userId)
            .OrderByDescending(s => s.id)
            .ToListAsync();

        var songDtos = songs.Select(s => new SongResDto
        {
            id = s.id,
            title = s.title,
            songKey = s.songKey,
            artist =  s.artist,
            image = s.image
        });

        return songDtos;
    }

    public async Task<IEnumerable<SongResDto>> GetSongs()
    {
        var songs = await context.Songs
            .Where(s => s.isPublic)
            .OrderByDescending(s => s.id)
            .ToListAsync();

        var songDtos = songs.Select(s => new SongResDto
        {
            id = s.id,
            title = s.title,
            songKey = s.songKey,
            artist = s.artist,
            image = s.image
        });
        
        return songDtos;
    }
}