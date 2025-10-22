using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces
{
    public interface IGenreRepository : IMusicRepository<Genre> , IRepository<Genre>
    {
    }
}
