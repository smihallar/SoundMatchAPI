using System.Text.Json.Serialization;

namespace SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses
{
    public class SpotifyTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; } // seconds until token expires

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;
    }
}
