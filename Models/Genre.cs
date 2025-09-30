namespace SoundMatchAPI.Models
{
    public class Genre
    {
        public string GenreId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
    }
}
