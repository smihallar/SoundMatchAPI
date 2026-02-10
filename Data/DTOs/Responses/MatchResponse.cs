using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.DTOs.Responses
{
    public class MatchResponse
    {
        public string MatchId { get; set; } = Guid.NewGuid().ToString();
        public required string InitiatorUserId { get; set; } // User who creates an account or refreshes matches.
        public required string RecipientUserId { get; set; } // User who is being matched with the initiator.
        public int CompatibilityScore { get; set; } // Score representing compatibility between users
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Song> MutualSongs { get; set; } = new List<Song>();
        public List<Artist> MutualArtists { get; set; } = new List<Artist>();
        public List<Genre> MutualGenres { get; set; } = new List<Genre>();
    }
}
