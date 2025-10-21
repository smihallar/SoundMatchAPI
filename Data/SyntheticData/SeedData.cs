using Microsoft.AspNetCore.Identity;
using SoundMatchAPI.Constants;
using SoundMatchAPI.Data.Models;
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
                };
                ctx.Roles.AddRange(roles);
                await ctx.SaveChangesAsync();
            }

            // Seed synthetic users from JSON
            if (!ctx.Users.Any(u => u.IsSynthetic))
            {
                var json = await File.ReadAllTextAsync("Data/SyntheticData/spotify_seed_data.json"); // JSON file with spotify data of artists, songs and genres
                var seedData = JsonDocument.Parse(json);

                // Example: create synthetic users with favorite artists
                var artists = seedData.RootElement.GetProperty("Artists");
                var artistList = new List<Artist>();
                foreach (var artistJson in artists.EnumerateArray())
                {
                    var artist = new Artist
                    {
                        ArtistId = artistJson.GetProperty("ArtistId").GetString()!,
                        Name = artistJson.GetProperty("name").GetString()!,
                        SpotifyId = artistJson.GetProperty("id").GetString()!,
                        Popularity = artistJson.GetProperty("popularity").GetInt32(),
                        ArtistImageUrl = artistJson.GetProperty("ArtistImageUrl").GetString()!,
                        Genres = artistJson.TryGetProperty("Genres", out var genresJson)
                            ? genresJson.EnumerateArray().Select(g => new Genre
                            {
                                GenreId = g.GetProperty("GenreId").GetString()!,
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
                            SongId = songJson.GetProperty("SongId").GetGuid().ToString(),
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
                    // Select favorite artists, genres, and songs
                    var favoriteArtists = artistList.OrderBy(_ => Guid.NewGuid()).Take(25).ToList();
                    var favoriteGenres = artistList
                        .SelectMany(a => a.Genres)
                        .Distinct()
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(10)
                        .ToList();
                    var favoriteSongs = songs.OrderBy(_ => Guid.NewGuid()).Take(60).ToList();

                    var user = new User
                    {
                        UserName = $"synthetic_user_{i}",
                        Email = $"synthetic_user_{i}@example.com",
                        CountryCode = "SE",
                        IsSynthetic = true,
                        ProfilePictureUrl = "https://static.vecteezy.com/system/resources/previews/009/292/244/non_2x/default-avatar-icon-of-social-media-user-vector.jpg",
                        FavoriteArtistIds = favoriteArtists.Select(a => a.ArtistId).ToList(),
                        FavoriteGenreIds = favoriteGenres.Select(g => g.GenreId).ToList(),
                        FavoriteSongIds = favoriteSongs.Select(s => s.SongId).ToList(),
                        FavoriteArtists = favoriteArtists,
                        FavoriteGenres = favoriteGenres,
                        FavoriteSongs = favoriteSongs,
                        IsConnectedToSpotify = true
                    };
                    ctx.Users.Add(user);
                }
                await ctx.SaveChangesAsync();

                // Fetch some users to have matching favorites to ensure matching logic
                var usersToMatch = ctx.Users
                    .OrderBy(u => u.UserName)
                    .Take(3)
                    .ToList();

                // Shared favorites (e.g., from existing songs/artists/genres)
                var sharedSongIds = ctx.Songs.Take(5).Select(s => s.SongId).ToList();
                var sharedArtistIds = ctx.Artists.Take(2).Select(a => a.ArtistId).ToList();
                var sharedGenreIds = ctx.Genres.Take(2).Select(g => g.GenreId).ToList();

                // Assign the shared favorites to these users
                foreach (var user in usersToMatch)
                {
                    user.FavoriteSongIds.AddRange(sharedSongIds);
                    user.FavoriteArtistIds.AddRange(sharedArtistIds);
                    user.FavoriteGenreIds.AddRange(sharedGenreIds);
                    user.FavoriteSongs.AddRange(ctx.Songs.Where(s => sharedSongIds.Contains(s.SongId)).ToList());
                    user.FavoriteArtists.AddRange(ctx.Artists.Where(a => sharedArtistIds.Contains(a.ArtistId)).ToList());
                    user.FavoriteGenres.AddRange(ctx.Genres.Where(g => sharedGenreIds.Contains(g.GenreId)).ToList());
                }
                await ctx.SaveChangesAsync();
            }
        }
    }
}
