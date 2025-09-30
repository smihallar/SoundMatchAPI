namespace SoundMatchAPI.Models
{
    public class Song
    {
        public Guid SongId { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string AlbumImageUrl { get; set; } = string.Empty;
        public int Popularity { get; set; } // Scale of 0-100, calculated by Spotify
        public string SpotifyId { get; set; } = string.Empty; // Song's Spotify ID


        // Navigation properties
        public List<Artist>? Artists { get; set; }
    }
}
