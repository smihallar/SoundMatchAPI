namespace SoundMatchAPI.Models
{
    public class Match
    {
        public Guid MatchId { get; set; } = Guid.NewGuid();
        public string InitiatorUserId { get; set; } // User who creates an account or refreshes matches.
        public string RecipientUserId { get; set; } // User who is being matched with the initiator.
        public int CompatibilityScore { get; set; } // Score representing compatibility between users
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User InitiatorUser { get; set; }
        public User RecipientUser { get; set; }

        public List<Song> MutualSongs { get; set; } // Songs that matched users share, 3 points each
        public List<Artist> MutualArtists { get; set; } // Artists that matched users share, 2 points each
        public List<Genre> MutualGenres { get; set; } // Genres that matched users share, 1 point each
    }
}
