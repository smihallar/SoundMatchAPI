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
        public List<string> MutualSongIds { get; set; } = new List<string>();
        public List<string> MutualArtistIds { get; set; } = new List<string>();
        public List<string> MutualGenreIds { get; set; } = new List<string>();
    }
}
