using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.RepositoryInterfaces
{
    public interface IArtistRepository : IMusicRepository<Artist>, IRepository<Artist>
    {
        Task<Artist?> GetArtistWithDetailsAsync(string id);
        Task<Artist?> GetBySpotifyIdAsync(string spotifyId);
    }
}
