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
            //Build a cache of all existing genres
            var allGenreNames = profile.Genres
                .Where(g => !string.IsNullOrEmpty(g.Name))
                .Select(g => g.Name)
                .Distinct()
                .ToList();

            var existingGenres = await genreRepository.GetByNamesAsync(allGenreNames);
            var genreLookup = existingGenres.ToDictionary(g => g.Name, g => g, StringComparer.OrdinalIgnoreCase);

            // Upsert missing genres
            foreach (var genre in profile.Genres.Where(g => !string.IsNullOrEmpty(g.Name)))
            {
                if (!genreLookup.TryGetValue(genre.Name, out var trackedGenre) || trackedGenre == null)
                {
                    await genreRepository.AddAsync(genre);
                    genreLookup[genre.Name] = genre;
                }
                else
                {
                    genre.GenreId = trackedGenre.GenreId;
                }
            }

            // 3Upsert artists
            foreach (var artist in profile.Artists)
            {
                var existingArtist = await artistRepository.GetBySpotifyIdAsync(artist.SpotifyId);

                // Attach tracked genres safely
                artist.Genres = (artist.Genres ?? new List<Genre>())
                    .Where(g => !string.IsNullOrEmpty(g.Name) && genreLookup.ContainsKey(g.Name))
                    .Select(g => genreLookup[g.Name])
                    .ToList();

                if (existingArtist == null)
                {
                    await artistRepository.AddAsync(artist);
                }
                else
                {
                    bool needsUpdate = false;

                    if (existingArtist.Popularity != artist.Popularity)
                    {
                        existingArtist.Popularity = artist.Popularity;
                        needsUpdate = true;
                    }

                    if (existingArtist.ArtistImageUrl != artist.ArtistImageUrl)
                    {
                        existingArtist.ArtistImageUrl = artist.ArtistImageUrl;
                        needsUpdate = true;
                    }

                    // Add missing genres
                    foreach (var g in artist.Genres)
                    {
                        if (!existingArtist.Genres.Any(x => x.GenreId == g.GenreId))
                        {
                            existingArtist.Genres.Add(g);
                            needsUpdate = true;
                        }
                    }

                    if (needsUpdate)
                        await artistRepository.UpdateAsync(existingArtist);
                }
            }

            // Upsert songs
            foreach (var song in profile.Songs)
            {
                var existingSong = await songRepository.GetBySpotifyIdAsync(song.SpotifyId);
                bool isNewSong = existingSong == null;

                // Track artists safely
                var trackedArtists = new List<Artist>();
                foreach (var artist in song.Artists ?? new List<Artist>())
                {
                    var tracked = await artistRepository.GetBySpotifyIdAsync(artist.SpotifyId);
                    if (tracked == null)
                    {
                        // Attach genres safely
                        artist.Genres = (artist.Genres ?? new List<Genre>())
                            .Where(g => !string.IsNullOrEmpty(g.Name) && genreLookup.ContainsKey(g.Name))
                            .Select(g => genreLookup[g.Name])
                            .ToList();

                        await artistRepository.AddAsync(artist);
                        tracked = artist;
                    }

                    trackedArtists.Add(tracked);
                }

                if (isNewSong)
                {
                    song.Artists = trackedArtists;
                    await songRepository.AddAsync(song);
                }
                else
                {
                    bool needsUpdate = false;

                    if (string.IsNullOrEmpty(existingSong.AlbumImageUrl) || existingSong.AlbumImageUrl != song.AlbumImageUrl)
                    {
                        existingSong.AlbumImageUrl = song.AlbumImageUrl;
                        needsUpdate = true;
                    }

                    if (existingSong.Popularity != song.Popularity)
                    {
                        existingSong.Popularity = song.Popularity;
                        needsUpdate = true;
                    }

                    // Update artist relationships
                    var existingArtistIds = existingSong.Artists.Select(a => a.ArtistId).OrderBy(x => x).ToList();
                    var newArtistIds = trackedArtists.Select(a => a.ArtistId).OrderBy(x => x).ToList();

                    if (!existingArtistIds.SequenceEqual(newArtistIds))
                    {
                        existingSong.Artists.Clear();
                        existingSong.Artists.AddRange(trackedArtists);
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                        await songRepository.UpdateAsync(existingSong);
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
