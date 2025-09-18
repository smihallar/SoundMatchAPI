namespace SoundMatchAPI.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public List<Artist> Artists { get; set; } = new List<Artist>();
    }
}
