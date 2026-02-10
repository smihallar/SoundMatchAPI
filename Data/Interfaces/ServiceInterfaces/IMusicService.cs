using SoundMatchAPI.Data.DomainModels;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.ServiceInterfaces
{
    public interface IMusicService
    {
        Task<MusicProfile> GetProfileAsync(List<string> favoriteSongIds, List<string> favoriteArtistIds,
            List<string> favoriteGenreIds);
        Task SaveSpotifyMusicAsync(MusicProfile profile);

        Task<List<Song>?> GetSongBySpotifyIdsAsync(List<string> spotifyIds);
        Task<List<Artist>?> GetArtistBySpotifyIdsAsync(List<string> spotifyIds);
        Task<List<Genre>?> GetGenreByNamesAsync(List<string> names);
    }
}
