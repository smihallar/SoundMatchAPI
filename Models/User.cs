using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundMatchAPI.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string SpotifyUserId { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty; // ex. "US", "GB"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsSynthetic { get; set; }

        // Navigation properties
        public List<Song> FavoriteSongs { get; set; } = new List<Song>();
        public List<Artist> FavoriteArtists { get; set; } = new List<Artist>();
        public List<Genre> FavoriteGenres { get; set; } = new List<Genre>();
        public List<Match> MatchesAsInitiator { get; set; } = new List<Match>();
        public List<Match> MatchesAsRecipient { get; set; } = new List<Match>();

        // Combined Matches-property to get all matches regardless of role (initiator or recipient). Not mapped to DB
        [NotMapped]
        public IEnumerable<(Match Match, bool IsInitiator)> AllMatches =>
                     MatchesAsInitiator.Select(m => (m, true))
                    .Concat(MatchesAsRecipient.Select(m => (m, false)));
    }
}
