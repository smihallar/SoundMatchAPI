using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.ServiceInterfaces
{
    public interface ISpotifyAuthService
    {
        Task<ReturnResponse<SpotifyAuthorizationUrlResponse>> GetAuthorizationUrl(string userId);
        Task<ReturnResponse<SpotifyTokenResponse>> ExchangeCodeAndStoreTokensAsync(User user, string code);
        Task<ReturnResponse<SpotifyTokenResponse>> GetAccessTokenAsync(User user);
    }
}
