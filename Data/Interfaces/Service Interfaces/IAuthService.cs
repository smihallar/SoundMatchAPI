using SoundMatchAPI.Data.AuthModels;
using SoundMatchAPI.Data.DTOs.Requests;

namespace SoundMatchAPI.Data.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterUserAsync(UserRegisterRequest request);
        Task<AuthResult> LoginUserAsync(UserLoginRequest request);
    }
}
