using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces
{
    public interface IArtistRepository : IMusicRepository<Artist>, IRepository<Artist>
    {
        Task<Artist?> GetArtistWithDetailsAsync(string id);
    }
}
