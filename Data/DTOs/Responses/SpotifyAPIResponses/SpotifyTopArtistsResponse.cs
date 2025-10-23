using System.Text.Json.Serialization;

namespace SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses
{
    public class SpotifyTopArtistsResponse
    {
        [JsonPropertyName("items")]
        public List<SpotifyArtistResponse> Items { get; set; } = new List<SpotifyArtistResponse>();
    }
}
