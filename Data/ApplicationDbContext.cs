using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Models;

namespace SoundMatchAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Match> Matches { get; set; }

        // Configure entity relationships and constraints, and adds cascading deletes for matches
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.InitiatorUser)
                .WithMany(u => u.MatchesAsInitiator)
                .HasForeignKey(m => m.InitiatorUserId)
                .OnDelete(DeleteBehavior.NoAction); // To prevent cascading delete cycles

            modelBuilder.Entity<Match>()
                .HasOne(m => m.RecipientUser)
                .WithMany(u => u.MatchesAsRecipient)
                .HasForeignKey(m => m.RecipientUserId)
                .OnDelete(DeleteBehavior.NoAction); // To prevent cascading delete cycles

            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteSongs)
                .WithMany(); // Unidirectional relationship

            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteArtists)
                .WithMany(); // Unidirectional

            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteGenres)
                .WithMany(); // Unidirectional

            modelBuilder.Entity<Song>()
                .HasMany(s => s.Artists)
                .WithMany(a => a.Songs); // Unidirectional

            modelBuilder.Entity<Artist>()
                .HasMany(s => s.Genres)
                .WithMany(g => g.Artists);
        }
    }
}
