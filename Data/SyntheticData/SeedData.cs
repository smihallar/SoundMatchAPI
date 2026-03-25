 using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Constants;
using SoundMatchAPI.Data.Models;
using System.Text.Json;

namespace SoundMatchAPI.Data.SyntheticData
{
    public class SeedData
    {
        public static async Task Initialize(ApplicationDbContext ctx)
        {
            // Ensure roles exist
            if (!ctx.Roles.Any())
            {
                ctx.Roles.Add(new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "User",
                    NormalizedName = ApiRoles.User
                });
                await ctx.SaveChangesAsync();
            }

            // Do not seed again if synthetic users already exist
            if (ctx.Users.Any(u => u.IsSynthetic))
                return;

            var hasher = new PasswordHasher<User>();

            var json = await File.ReadAllTextAsync("Data/SyntheticData/spotify_seed_data.json");
            var seedData = JsonDocument.Parse(json);

            // Load artists first
            var artistLookup = new Dictionary<string, Artist>();
            var genreLookup = new Dictionary<string, Genre>(StringComparer.OrdinalIgnoreCase);

            var artistsJson = seedData.RootElement.GetProperty("Artists");
            foreach (var artistJson in artistsJson.EnumerateArray())
            {
                var spotifyId = artistJson.GetProperty("id").GetString()!;
                var name = artistJson.GetProperty("name").GetString()!;

                var artist = new Artist
                {
                    ArtistId = Guid.NewGuid().ToString(),
                    Name = name,
                    SpotifyId = spotifyId,
                    Popularity = artistJson.GetProperty("popularity").GetInt32(),
                    ArtistImageUrl = artistJson.GetProperty("ArtistImageUrl").GetString()!,
                    Genres = new List<Genre>()
                };

                // Load genres & de-duplicate
                if (artistJson.TryGetProperty("Genres", out var genresJson))
                {
                    foreach (var g in genresJson.EnumerateArray())
                    {
                        var genreName = g.GetProperty("Name").GetString()!;
                        if (!genreLookup.TryGetValue(genreName, out var existingGenre))
                        {
                            existingGenre = new Genre
                            {
                                GenreId = Guid.NewGuid().ToString(),
                                Name = genreName
                            };
                            genreLookup[genreName] = existingGenre;
                        }
                        artist.Genres.Add(existingGenre);
                    }
                }

                artistLookup[spotifyId] = artist;
            }

            ctx.Genres.AddRange(genreLookup.Values);
            ctx.Artists.AddRange(artistLookup.Values);
            await ctx.SaveChangesAsync();

            // Load songs and connect existing artists
            var songs = new List<Song>();
            var songLookup = new HashSet<string>();

            if (seedData.RootElement.TryGetProperty("Songs", out var songsJson))
            {
                foreach (var songJson in songsJson.EnumerateArray())
                {
                    var spotifyId = songJson.GetProperty("SpotifyId").GetString()!;

                    if (songLookup.Contains(spotifyId))
                        continue;

                    var song = new Song
                    {
                        SongId = Guid.NewGuid().ToString(),
                        Title = songJson.GetProperty("Title").GetString()!,
                        AlbumImageUrl = songJson.GetProperty("AlbumImageUrl").GetString()!,
                        Popularity = songJson.GetProperty("Popularity").GetInt32(),
                        SpotifyId = spotifyId,
                        Artists = new List<Artist>()
                    };

                    if (songJson.TryGetProperty("Artists", out var songArtistsJson))
                    {
                        foreach (var a in songArtistsJson.EnumerateArray())
                        {
                            var artistSpotifyId = a.GetProperty("id").GetString()!;
                            if (artistLookup.TryGetValue(artistSpotifyId, out var artist))
                            {
                                song.Artists.Add(artist);
                            }
                        }
                    }

                    songLookup.Add(spotifyId);
                    songs.Add(song);
                }
            }

            ctx.Songs.AddRange(songs);
            await ctx.SaveChangesAsync();

            // Create synthetic users
            var allArtists = artistLookup.Values.ToList();
            var allGenres = genreLookup.Values.ToList();

            for (int i = 0; i < 50; i++)
            {
                var favoriteArtists = allArtists.OrderBy(_ => Guid.NewGuid()).Take(30).ToList();
                var favoriteGenres = allGenres.OrderBy(_ => Guid.NewGuid()).Take(10).ToList();
                var favoriteSongs = songs.OrderBy(_ => Guid.NewGuid()).Take(50).ToList();

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = $"synthetic_user_{i}",
                    NormalizedUserName = $"SYNTHETIC_USER_{i}",
                    Email = $"synthetic_user_{i}@example.com",
                    NormalizedEmail = $"SYNTHETIC_USER_{i}@EXAMPLE.COM",
                    CountryCode = "SE",
                    IsSynthetic = true,
                    IsConnectedToSpotify = true,
                    ProfilePictureUrl = "https://static.vecteezy.com/system/resources/previews/009/292/244/non_2x/default-avatar-icon-of-social-media-user-vector.jpg",
                    FavoriteArtists = favoriteArtists,
                    FavoriteArtistIds = favoriteArtists.Select(a => a.ArtistId).ToList(),
                    FavoriteGenres = favoriteGenres,
                    FavoriteGenreIds = favoriteGenres.Select(g => g.GenreId).ToList(),
                    FavoriteSongs = favoriteSongs,
                    FavoriteSongIds = favoriteSongs.Select(s => s.SongId).ToList(),
                    MusicTasteLastRefreshed = DateTime.UtcNow
                };

                user.PasswordHash = hasher.HashPassword(user, "Synthetic123!");
                ctx.Users.Add(user);
            }

            await ctx.SaveChangesAsync();
        }
    }
}
