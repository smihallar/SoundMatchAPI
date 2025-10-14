namespace SoundMatchAPI.Data.Models
{
    public class Match
    {
        public string MatchId { get; set; } = Guid.NewGuid().ToString();
        public required string InitiatorUserId { get; set; } // User who creates an account or refreshes matches.
        public required string RecipientUserId { get; set; } // User who is being matched with the initiator.
        public int CompatibilityScore { get; set; } // Score representing compatibility between users
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public required User InitiatorUser { get; set; }
        public required User RecipientUser { get; set; }

        public List<Song>? MutualSongs { get; set; } = new List<Song>(); // Songs that matched users share, 3 points each
        public List<Artist>? MutualArtists { get; set; } = new List<Artist>(); // Artists that matched users share, 2 points each
        public List<Genre>? MutualGenres { get; set; } = new List<Genre>(); // Genres that matched users share, 1 point each
    }
}
