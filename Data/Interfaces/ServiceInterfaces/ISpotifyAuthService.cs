using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.ServiceInterfaces
{
    public interface ISpotifyAuthService
    {
        Task<ReturnResponse<string>> GetAuthorizationUrl();
        Task<ReturnResponse<string>> ExchangeCodeAndStoreTokensAsync(User user, string code);
        Task<ReturnResponse<string>> GetAccessTokenAsync(User user);
    }
}
