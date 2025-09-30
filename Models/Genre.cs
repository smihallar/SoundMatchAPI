namespace SoundMatchAPI.Models
{
    public class Genre
    {
        public Guid GenreId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }
}
