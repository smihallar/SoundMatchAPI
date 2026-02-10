using SoundMatchAPI.Data.Models;
using System.Text.Json.Serialization;

namespace SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses
{
    public class SpotifyTrackResponse // Top tracks from Spotify API
    {
        [JsonPropertyName("id")]
        public string SpotifyId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("album")]
        public SpotifyAlbumResponse Album { get; set; } = new();

        [JsonPropertyName("artists")]
        public List<SpotifyArtistResponse> Artists { get; set; } = new();

        [JsonPropertyName("popularity")]
        public int Popularity { get; set; }
    }
}
