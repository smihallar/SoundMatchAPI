using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.ServiceInterfaces
{
    public interface ISpotifyDataService
    {
        Task<ReturnResponse> ConnectSpotifyAndPopulateMusicAsync(User user, string accessToken); // Used during initial connection
        Task<ReturnResponse<UserProfileResponse>> RefreshTopItemsAsync(User user, string accessToken); // Used to refresh user's top items
        Task<ReturnResponse<UserProfileResponse>> RefreshUserProfileAsync(User user, string accessToken); // Used to refresh user's profile data
    }
}
