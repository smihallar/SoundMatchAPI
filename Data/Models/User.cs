using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundMatchAPI.Data.Models
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
        public string Biography { get; set; } = string.Empty;
        public bool IsSynthetic { get; set; }
        public bool IsConnectedToSpotify { get; set; } // Indicates if the user has connected their Spotify account
        public DateTime MusicTasteLastRefreshed { get; set; } = DateTime.MinValue; // When the user's music taste was last refreshed from Spotify, stored to avoid excessive Spotify API calls
        public DateTime UserDetailsLastRefreshed { get; set; } = DateTime.MinValue; // When the user's details were last refreshed from Spotify

        [ProtectedPersonalData]
        [PersonalData]
        public string? SpotifyRefreshToken { get; set; } // Refresh token for Spotify API, used to get new access tokens
        [ProtectedPersonalData]
        [PersonalData]
        public DateTime? SpotifyTokenExpiresAt { get; set; } // When the current Spotify token expires


        // Navigation properties 
        public List<string> FavoriteSongIds { get; set; } = new List<string>();
        public List<Song> FavoriteSongs { get; set; } = new List<Song>();

        public List<string> FavoriteArtistIds { get; set; } = new List<string>();
        public List<Artist> FavoriteArtists { get; set; } = new List<Artist>();

        public List<string> FavoriteGenreIds { get; set; } = new List<string>();
        public List<Genre> FavoriteGenres { get; set; } = new List<Genre>();

        public List<string> MatchIdsAsInitiator { get; set; } = new List<string>();
        public List<Match> MatchesAsInitiator { get; set; } = new List<Match>();

        public List<string> MatchIdsAsRecipient { get; set; } = new List<string>();
        public List<Match> MatchesAsRecipient { get; set; } = new List<Match>();

        public List<string> ChatIds { get; set; } = new List<string>();
        public List<Chat> Chats { get; set; } = new List<Chat>();

        // Combined Matches-property to get all matchIds regardless of role (initiator or recipient). Not mapped to DB
        [NotMapped]
        public IEnumerable<string> AllMatchIds =>
            MatchIdsAsInitiator.Concat(MatchIdsAsRecipient);
    }
}
