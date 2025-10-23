using SoundMatchAPI.Data.DomainModels;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Repositories;

namespace SoundMatchAPI.Services
{
    public class MusicService : IMusicService
    {
        private readonly ISongRepository songRepository;
        private readonly IGenreRepository genreRepository;
        private readonly IArtistRepository artistRepository;

        public MusicService(ISongRepository songRepository, IGenreRepository genreRepository,
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

        // Save Spotify music data into the database, avoiding duplicates
        public async Task SaveSpotifyMusicAsync(MusicProfile profile)
        {
            // 1️⃣ Upsert genres
            foreach (var genre in profile.Genres)
            {
                var existingGenre = await genreRepository.GetByNameAsync(genre.Name);
                if (existingGenre == null)
                {
                    await genreRepository.AddAsync(genre);
                }
            }

            // 2️⃣ Upsert artists
            foreach (var artist in profile.Artists)
            {
                var existingArtist = await artistRepository.GetBySpotifyIdAsync(artist.SpotifyId);
                if (existingArtist == null)
                {
                    await artistRepository.AddAsync(artist);
                }
            }

            // 3️⃣ Upsert songs
            foreach (var song in profile.Songs)
            {
                var existingSong = await songRepository.GetBySpotifyIdAsync(song.SpotifyId);
                if (existingSong == null)
                {
                    await songRepository.AddAsync(song);
                }
            }
        }
    }
}
