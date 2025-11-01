using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.RepositoryInterfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task UpdateUserWithSpotifyAuthDetailsAsync(string userId, string? refreshToken, DateTime? tokenExpiresAt);
        Task<User?> GetUserWithFavoriteMusic(string userId);
    }
}
