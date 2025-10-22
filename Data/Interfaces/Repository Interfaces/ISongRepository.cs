using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces
{
    public interface ISongRepository : IMusicRepository<Song> , IRepository<Song>
    {
        Task<Song?> GetSongWithDetailsAsync(string id);
        Task<IEnumerable<Song?>> GetSongsWithDetailsByIdsAsync(IEnumerable<string> songIds);
    }
}
