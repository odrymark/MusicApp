using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Song
{
    public Guid id { get; set; }

    [Required]
    public Guid userId { get; set; }

    [Required]
    [MaxLength(100)]
    public string title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string songKey { get; set; } = string.Empty;

    public string? image { get; set; }

    [ForeignKey(nameof(userId))]
    public User user { get; set; } = null!;

    public ICollection<Playlist> playlists { get; set; } = new List<Playlist>();
}