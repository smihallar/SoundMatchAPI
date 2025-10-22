using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        //Task<IEnumerable<User?>> GetAllUsersWithDetailsAsync();
        //Task<User?> GetUserWithDetailsAsync(string id);
    }
}
