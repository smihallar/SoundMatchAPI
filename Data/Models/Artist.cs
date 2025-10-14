using System.ComponentModel.DataAnnotations;

namespace SoundMatchAPI.Data.Models
{
    public class Artist
    {
        public string ArtistId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string ArtistImageUrl { get; set; } = string.Empty;
        public string SpotifyId { get; set; } = string.Empty; // Artist's Spotify ID
        public int? Popularity { get; set; } // Scale of 0-100, calculated by Spotify

        // Navigation property
        public List<Genre> Genres { get; set; } = new List<Genre>();

    }
}
