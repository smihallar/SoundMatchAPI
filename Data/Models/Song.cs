namespace SoundMatchAPI.Data.Models
{
    public class Song
    {
        public string SongId { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string AlbumImageUrl { get; set; } = string.Empty;
        public int? Popularity { get; set; } // Scale of 0-100, calculated by Spotify
        public string SpotifyId { get; set; } = string.Empty; // Song's Spotify ID


        // Navigation properties
        public List<Artist> Artists { get; set; } = new List<Artist>();
    }
}
