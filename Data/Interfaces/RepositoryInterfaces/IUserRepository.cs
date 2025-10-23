using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.RepositoryInterfaces
{
    public interface IUserRepository : IRepository<User>
    {
       Task UpdateUserSpotifyStatusAsync(string userId, bool isConnectedToSpotify);
    }
}
