using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class MusicDbContext : DbContext
{
    public MusicDbContext(DbContextOptions<MusicDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Song>()
            .HasOne(s => s.User)
            .WithMany(u => u.Songs)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Playlist>()
            .HasOne(p => p.User)
            .WithMany(u => u.Playlists)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Playlist>()
            .HasMany(p => p.Songs)
            .WithMany(s => s.Playlists)
            .UsingEntity<Dictionary<string, object>>(
                "PlaylistSongs",
                j => j
                    .HasOne<Song>()
                    .WithMany()
                    .HasForeignKey("SongId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<Playlist>()
                    .WithMany()
                    .HasForeignKey("PlaylistId")
                    .OnDelete(DeleteBehavior.Restrict)
            );
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
}