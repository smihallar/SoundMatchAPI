using SoundMatchAPI.Data.DomainModels;
using SoundMatchAPI.Data.Interfaces;
using SoundMatchAPI.Data.Repositories;

namespace SoundMatchAPI.Services
{
    public class MusicProfileService
    {
        private readonly ISongRepository songRepository;
        private readonly IGenreRepository genreRepository;
        private readonly IArtistRepository artistRepository;

        public MusicProfileService(ISongRepository songRepository, IGenreRepository genreRepository, 
            IArtistRepository artistRepository)
        {
            this.songRepository = songRepository;
            this.genreRepository = genreRepository;
            this.artistRepository = artistRepository;
        }

        public async Task<MusicProfile> GetProfileAsync(List<string> favoriteSongIds, List<string> favoriteGenreIds,
            List<string> favoriteArtistIds)
        {
            var songs = await songRepository.GetByIdsAsync(favoriteSongIds);
            var genres = await genreRepository.GetByIdsAsync(favoriteGenreIds);
            var artists = await artistRepository.GetByIdsAsync(favoriteArtistIds);

            return new MusicProfile(songs, genres, artists);
        }
    }
}
