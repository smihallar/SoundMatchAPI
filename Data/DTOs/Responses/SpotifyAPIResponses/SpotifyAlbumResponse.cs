using System.Text.Json.Serialization;

namespace SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses
{
    public class SpotifyAlbumResponse // Album info from Spotify API (song's album)
    {
        [JsonPropertyName("images")]
        public List<SpotifyImageResponse> Images { get; set; } = new();
    }
}
