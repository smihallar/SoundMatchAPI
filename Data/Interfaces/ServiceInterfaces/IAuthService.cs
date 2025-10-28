using SoundMatchAPI.Data.AuthModels;
using SoundMatchAPI.Data.DTOs.Requests;
using SoundMatchAPI.Data.DTOs.Responses;

namespace SoundMatchAPI.Data.Interfaces.ServiceInterfaces
{
    public interface IAuthService
    {
        Task<ReturnResponse<AuthResponse>> RegisterUserAsync(UserRegisterRequest request);
        Task<ReturnResponse<AuthResponse>> LoginUserAsync(UserLoginRequest request);
    }
}
