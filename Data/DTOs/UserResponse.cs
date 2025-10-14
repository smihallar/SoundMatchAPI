namespace SoundMatchAPI.Data.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty; // Spotify username
        public string Email { get; set; } = string.Empty;
        public string SpotifyUserId { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty; // ex. "US", "GB"
        public DateTime CreatedAt { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public bool IsSynthetic { get; set; }
        public List<string> FavoriteSongIds { get; set; } = new List<string>();
        public List<string> FavoriteArtistIds { get; set; } = new List<string>();
        public List<string> FavoriteGenreIds { get; set; } = new List<string>();




    }
}
