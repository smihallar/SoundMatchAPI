using Microsoft.AspNetCore.Identity;
using SoundMatchAPI.Constants;
using SoundMatchAPI.Models;
using System.Text.Json;

namespace SoundMatchAPI.Data.SyntheticData
{
    public class SeedData
    {
        public static async Task Initialize(ApplicationDbContext ctx)
        {
            if (!ctx.Roles.Any())
            {
                var roles = new List<IdentityRole>
                {
                    new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "User", NormalizedName = ApiRoles.User },
                    new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin", NormalizedName = ApiRoles.Admin }
                };
                ctx.Roles.AddRange(roles);
                await ctx.SaveChangesAsync();
            }

            // Seed synthetic users from JSON
            if (!ctx.Users.Any(u => u.IsSynthetic))
            {
                var json = await File.ReadAllTextAsync("Data/SyntheticData/spotify_seed_data.json");
                var seedData = JsonDocument.Parse(json);

                // Example: create synthetic users with favorite artists
                var artists = seedData.RootElement.GetProperty("Artists");
                var artistList = new List<Artist>();
                foreach (var artistJson in artists.EnumerateArray())
                {
                    var artist = new Artist
                    {
                        ArtistId = Guid.Parse(artistJson.GetProperty("ArtistId").GetString()!),
                        Name = artistJson.GetProperty("name").GetString()!,
                        SpotifyId = artistJson.GetProperty("id").GetString()!,
                        Popularity = artistJson.GetProperty("popularity").GetInt32(),
                        ArtistImageUrl = artistJson.GetProperty("ArtistImageUrl").GetString()!,
                        Genres = artistJson.TryGetProperty("Genres", out var genresJson)
                            ? genresJson.EnumerateArray().Select(g => new Genre
                            {
                                GenreId = Guid.Parse(g.GetProperty("GenreId").GetString()!),
                                Name = g.GetProperty("Name").GetString()!
                            }).ToList()
                            : new List<Genre>()
                    };
                    artistList.Add(artist);
                }

                // Add artists and genres to context if not present
                foreach (var artist in artistList)
                {
                    if (!ctx.Artists.Any(a => a.SpotifyId == artist.SpotifyId))
                        ctx.Artists.Add(artist);

                    foreach (var genre in artist.Genres)
                    {
                        if (!ctx.Genres.Any(g => g.GenreId == genre.GenreId))
                            ctx.Genres.Add(genre);
                    }
                }
                await ctx.SaveChangesAsync();

                // Parse songs
                var songs = new List<Song>();
                if (seedData.RootElement.TryGetProperty("Songs", out var songsJson))
                {
                    foreach (var songJson in songsJson.EnumerateArray())
                    {
                        var song = new Song
                        {
                            SongId = songJson.GetProperty("SongId").GetGuid(),
                            Title = songJson.GetProperty("Title").GetString() ?? "",
                            AlbumImageUrl = songJson.GetProperty("AlbumImageUrl").GetString() ?? "",
                            Popularity = songJson.GetProperty("Popularity").GetInt32(),
                            SpotifyId = songJson.GetProperty("SpotifyId").GetString() ?? "",
                            Artists = songJson.TryGetProperty("Artists", out var songArtistsJson)
                            ? songArtistsJson.EnumerateArray()
                                .Select(a =>
                                    artistList.FirstOrDefault(x =>
                                        x.ArtistId.ToString() == a.GetProperty("ArtistId").GetString())
                                )
                                .Where(a => a != null)
                                .ToList()!
                            : new List<Artist>()
                        };
                        songs.Add(song);
                    }
                }

                // Add songs to context if not present
                foreach (var song in songs)
                {
                    if (!ctx.Songs.Any(s => s.SpotifyId == song.SpotifyId))
                        ctx.Songs.Add(song);
                }
                await ctx.SaveChangesAsync();

                // Create synthetic users
                for (int i = 0; i < 150; i++)
                {
                    var user = new User
                    {
                        UserName = $"synthetic_user_{i}",
                        Email = $"synthetic_user_{i}@example.com",
                        CountryCode = "SE",
                        IsSynthetic = true,
                        ProfilePictureUrl = "https://static.vecteezy.com/system/resources/previews/009/292/244/non_2x/default-avatar-icon-of-social-media-user-vector.jpg",
                        FavoriteArtists = artistList.OrderBy(_ => Guid.NewGuid()).Take(25).ToList(),
                        FavoriteGenres = artistList.SelectMany(a => a.Genres).Distinct().OrderBy(_ => Guid.NewGuid()).Take(10).ToList(),
                        FavoriteSongs = songs.OrderBy(_ => Guid.NewGuid()).Take(60).ToList()
                    };
                    ctx.Users.Add(user);
                }
                await ctx.SaveChangesAsync();
            }
        }
    }
}
