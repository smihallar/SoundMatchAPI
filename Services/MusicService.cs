using SoundMatchAPI.Data.DomainModels;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;
using SoundMatchAPI.Data.Repositories;
using System.Security.Permissions;

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
            // Upsert genres
            // 1️⃣ Build a cache of all existing genres
            var allGenreNames = profile.Genres.Select(g => g.Name).Distinct().ToList();
            var existingGenres = await genreRepository.GetByNamesAsync(allGenreNames);
            var genreLookup = existingGenres.ToDictionary(g => g.Name, g => g, StringComparer.OrdinalIgnoreCase);

            // 2️⃣ Upsert missing genres
            foreach (var genre in profile.Genres)
            {
                if (!genreLookup.TryGetValue(genre.Name, out var trackedGenre))
                {
                    await genreRepository.AddAsync(genre);
                    genreLookup[genre.Name] = genre; // Add it to the cache for reuse
                }
                else
                {
                    genre.GenreId = trackedGenre.GenreId;
                }
            }

            // 3️⃣ When attaching genres to artists
            foreach (var artist in profile.Artists)
            {
                var existingArtist = await artistRepository.GetBySpotifyIdAsync(artist.SpotifyId);
                if (existingArtist == null)
                {
                    // Replace artist.Genres with cached/tracked instances
                    artist.Genres = artist.Genres
                        .Where(g => genreLookup.ContainsKey(g.Name))
                        .Select(g => genreLookup[g.Name])
                        .ToList();

                    await artistRepository.AddAsync(artist);
                }
                else
                {
                    bool needsUpdate = false;

                    // Update image, popularity, etc.
                    if (existingArtist.Popularity != artist.Popularity)
                    {
                        existingArtist.Popularity = artist.Popularity;
                        needsUpdate = true;
                    }

                    // Add missing genres
                    foreach (var g in artist.Genres)
                    {
                        if (genreLookup.TryGetValue(g.Name, out var trackedGenre))
                        {
                            if (!existingArtist.Genres.Any(x => x.GenreId == trackedGenre.GenreId))
                            {
                                existingArtist.Genres.Add(trackedGenre);
                                needsUpdate = true;
                            }
                        }
                    }

                    if (needsUpdate)
                        await artistRepository.UpdateAsync(existingArtist);
                }
            }

            // Upsert songs
            // 3️⃣ Upsert songs
            foreach (var song in profile.Songs)
            {
                var existingSong = await songRepository.GetBySpotifyIdAsync(song.SpotifyId);
                bool isNewSong = existingSong == null;

                // Always use tracked artist instances
                var trackedArtists = new List<Artist>();
                foreach (var artist in song.Artists)
                {
                    var tracked = await artistRepository.GetBySpotifyIdAsync(artist.SpotifyId);
                    if (tracked != null)
                        trackedArtists.Add(tracked);
                }

                if (isNewSong)
                {
                    // Attach tracked artists
                    song.Artists = trackedArtists;

                    await songRepository.AddAsync(song);
                }
                else
                {
                    bool needsUpdate = false;

                    // Update album art if changed
                    if (string.IsNullOrEmpty(existingSong.AlbumImageUrl) ||
                        existingSong.AlbumImageUrl != song.AlbumImageUrl)
                    {
                        existingSong.AlbumImageUrl = song.AlbumImageUrl;
                        needsUpdate = true;
                    }

                    // Update popularity if changed
                    if (existingSong.Popularity != song.Popularity)
                    {
                        existingSong.Popularity = song.Popularity;
                        needsUpdate = true;
                    }

                    // Compare and update artist relationships
                    var existingArtistIds = existingSong.Artists.Select(a => a.ArtistId).OrderBy(x => x).ToList();
                    var newArtistIds = trackedArtists.Select(a => a.ArtistId).OrderBy(x => x).ToList();

                    if (!existingArtistIds.SequenceEqual(newArtistIds))
                    {
                        // Replace with tracked artist references
                        existingSong.Artists.Clear();
                        foreach (var a in trackedArtists)
                            existingSong.Artists.Add(a);

                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        await songRepository.UpdateAsync(existingSong);
                    }
                }
            }
        }

        public async Task<List<Song>?> GetSongBySpotifyIdsAsync(List<string> spotifyIds)
        {
            var songs = new List<Song>();
            foreach (var id in spotifyIds)
            {
                var song = await songRepository.GetBySpotifyIdAsync(id);
                if (song != null)
                {
                    songs.Add(song);
                }
            }
            return songs;
        }

        public async Task<List<Artist>?> GetArtistBySpotifyIdsAsync(List<string> spotifyIds)
        {
            var artists = new List<Artist>();
            foreach (var id in spotifyIds)
            {
               var artist = await artistRepository.GetBySpotifyIdAsync(id);
               if (artist != null)
               {
                   artists.Add(artist);
               }
            }
            return artists;
        }

        public async Task<List<Genre>?> GetGenreByNamesAsync(List<string> names)
        {
            var genres = new List<Genre>();
            foreach (var name in names)
            {
                var genre = await genreRepository.GetByNameAsync(name);
                if (genre != null)
                {
                    genres.Add(genre);
                }
            }
            return genres;
        }
    }
}
