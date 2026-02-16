using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Playlist
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Image { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public ICollection<Song> Songs { get; set; } = new List<Song>();
}