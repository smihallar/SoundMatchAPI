namespace SoundMatchAPI.Models
{
    public class Song
    {
        public int SongId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AlbumImageUrl { get; set; } = string.Empty;

        // Navigation properties
        public List<Artist> Artists { get; set; }
    }
}
