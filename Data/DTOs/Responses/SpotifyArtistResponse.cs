using System.Text.Json.Serialization;

namespace SoundMatchAPI.Data.DTOs.Responses
{
    public class SpotifyArtistResponse // Artist info from Spotify API (song's artists)
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("popularity")]
        public int Popularity { get; set; }

        [JsonPropertyName("images")]
        public List<SpotifyImageResponse> Images { get; set; } = new();

        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; } = new();
    }
}
