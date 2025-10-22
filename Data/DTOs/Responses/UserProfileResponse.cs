using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.DTOs.Responses
{
    public class UserProfileResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty; // Spotify username
        public string Email { get; set; } = string.Empty;
        public string SpotifyUserId { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty; // ex. "US", "GB"
        public DateTime CreatedAt { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public bool IsSynthetic { get; set; }
        public bool IsConnectedToSpotify { get; set; }
        public DateTime MusicTasteLastRefreshed { get; set; }
        public List<Song> FavoriteSongs { get; set; } = new List<Song>();
        public List<Artist> FavoriteArtists { get; set; } = new List<Artist>();
        public List<Genre> FavoriteGenres { get; set; } = new List<Genre>();
    }
}
