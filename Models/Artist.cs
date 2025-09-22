namespace SoundMatchAPI.Models
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ArtistImageUrl { get; set; } = string.Empty;

        // Navigation property
        public List<Genre> Genres { get; set; } = new List<Genre>();

    }
}
