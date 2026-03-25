using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        // Configure entity relationships and constraints, and adds cascading deletes for matches
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Song>()
                .HasIndex(s => s.SpotifyId)
                .IsUnique();
            modelBuilder.Entity<Artist>()
                .HasIndex(a => a.SpotifyId)
                .IsUnique();
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

            modelBuilder.Entity<Match>()
                .HasMany(m => m.MutualSongs)
                .WithMany(); // Unidirectional relationship, matches know about songs, songs don't need to know about matches

            modelBuilder.Entity<Match>()
                .HasMany(m => m.MutualArtists)
                .WithMany();

            modelBuilder.Entity<Match>()
                .HasMany(m => m.MutualGenres)
                .WithMany();

            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteSongs)
                .WithMany();

            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteArtists)
                .WithMany();

            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteGenres)
                .WithMany();

            modelBuilder.Entity<Song>()
                .HasMany(s => s.Artists)
                .WithMany();

            modelBuilder.Entity<Artist>()
                .HasMany(s => s.Genres)
                .WithMany();
            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Participants)
                .WithMany(u => u.Chats);

            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Chat>()
                .HasOne<Match>()
                .WithOne()
                .HasForeignKey<Chat>(c => c.MatchId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();
        }
    }
}
