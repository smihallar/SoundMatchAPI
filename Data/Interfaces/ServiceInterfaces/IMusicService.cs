using SoundMatchAPI.Data.DomainModels;

namespace SoundMatchAPI.Data.Interfaces.ServiceInterfaces
{
    public interface IMusicService
    {
        Task<MusicProfile> GetProfileAsync(List<string> favoriteSongIds, List<string> favoriteArtistIds,
            List<string> favoriteGenreIds);
        Task SaveSpotifyMusicAsync(MusicProfile profile);
    }
}
