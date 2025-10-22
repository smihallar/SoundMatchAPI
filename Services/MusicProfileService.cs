using SoundMatchAPI.Data.DomainModels;
using SoundMatchAPI.Data.Interfaces;
using SoundMatchAPI.Data.Repositories;

namespace SoundMatchAPI.Services
{
    public class MusicProfileService : IMusicProfileService
    {
        private readonly ISongRepository songRepository;
        private readonly IGenreRepository genreRepository;
        private readonly IArtistRepository artistRepository;

        public MusicProfileService(ISongRepository songRepository, IGenreRepository genreRepository, 
            IArtistRepository artistRepository)
        {
            this.songRepository = songRepository;
            this.artistRepository = artistRepository;
            this.genreRepository = genreRepository;
        }

        public async Task<MusicProfile> GetProfileAsync(List<string> favoriteSongIds, List<string> favoriteArtistIds,
            List<string> favoriteGenreIds)
        {
            var songs = await songRepository.GetByIdsAsync(favoriteSongIds);
            var artists = await artistRepository.GetByIdsAsync(favoriteArtistIds);
            var genres = await genreRepository.GetByIdsAsync(favoriteGenreIds);

            return new MusicProfile(songs, artists, genres);
        }
    }
}
