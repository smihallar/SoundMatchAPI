using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.RepositoryInterfaces
{
    public interface IChatRepository : IRepository<Chat>
    {
        Task<List<Chat>> GetChatsByUserIdAsync(string userId);
        Task<Chat?> GetChatByMatchIdAsync(string matchId);
    }
}
