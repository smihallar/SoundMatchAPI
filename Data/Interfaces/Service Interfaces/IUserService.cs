using SoundMatchAPI.Data.DTOs.Responses;

namespace SoundMatchAPI.Data.Interfaces
{
    public interface IUserService
    {
        Task<ReturnResponse<IEnumerable<UserResponse>>> GetAllUsersAsync();
        Task<ReturnResponse<UserResponse>> GetUserByIdAsync(string userId);
        Task<ReturnResponse> DeleteUserAsync(string userId, string loggedInUserId);
        Task<ReturnResponse> ConnectUserToSpotifyAsync(string userId);
        Task<ReturnResponse<UserProfileResponse>> GetUserProfileAsync(string userId);

    }
}
