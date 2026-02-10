using System.Text.Json.Serialization;

namespace SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses
{
    public class SpotifyTopTracksResponse
    {
        [JsonPropertyName("items")]
        public List<SpotifyTrackResponse> Items { get; set; } = new List<SpotifyTrackResponse>();
    }
}
