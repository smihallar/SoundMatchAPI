using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.RepositoryInterfaces
{
    public interface IMatchRepository : IRepository<Match>
    {
        Task<IEnumerable<Match?>> GetMatchesWithDetailsByUserIdAsync(string userId);
        Task<Match?> GetMatchWithDetailsByIdAsync(string matchId);
        Task AddMatchesAsync(IEnumerable<Match> matches);
        Task DeleteMatchesByUserIdAsync(string userId);
    }
}
