using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.RepositoryInterfaces
{
    public interface IGenreRepository : IMusicRepository<Genre> , IRepository<Genre>
    {
    }
}
