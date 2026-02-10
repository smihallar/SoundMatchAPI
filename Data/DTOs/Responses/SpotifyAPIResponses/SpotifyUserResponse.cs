using System.Text.Json.Serialization;

namespace SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses
{
    public class SpotifyUserResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("images")]
        public List<SpotifyImageResponse>? Images { get; set; }
    }
}
