using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.RepositoryInterfaces
{
    public interface ISongRepository : IMusicRepository<Song> , IRepository<Song>
    {
        Task<Song?> GetSongWithDetailsAsync(string id);
        Task<IEnumerable<Song?>> GetSongsWithDetailsByIdsAsync(IEnumerable<string> songIds);
        Task<Song?> GetBySpotifyIdAsync(string spotifyId);
    }
}
