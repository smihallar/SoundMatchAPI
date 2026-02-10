using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.RepositoryInterfaces
{
    public interface IGenreRepository : IMusicRepository<Genre> , IRepository<Genre>
    {
        Task<Genre?> GetByNameAsync(string name); // Used to compare to JSON from Spotify
        Task<IEnumerable<Genre>> GetByNamesAsync(IEnumerable<string> genreNames);
    }
}
