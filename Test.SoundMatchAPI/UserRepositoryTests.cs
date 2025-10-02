using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using SoundMatchAPI.Data;
using SoundMatchAPI.Data.Repositories;
using SoundMatchAPI.Models;

namespace Test.SoundMatchAPI
{
    public class UserRepositoryTests : IDisposable
    {
        private static DbContextOptions<ApplicationDbContext> dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "SoundMatchTestDatabase")
            .Options;
        private ApplicationDbContext ctx;
        private UserRepository userRepository;

        public UserRepositoryTests()
        {
            ctx = new ApplicationDbContext(dbContextOptions);
            userRepository = new UserRepository(ctx);
            ctx.Database.EnsureCreated();

            SeedDatabase(); // Seed the in-memory database with test data
        }

        private void SeedDatabase()
        {
            ctx.Genres.AddRange(
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Rock" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Metal" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Pop" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Dance" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Jazz" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Blues" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Classical" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Instrumental" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Hip-Hop" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Rap" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Electronic" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Country" },
                new Genre { GenreId = Guid.NewGuid().ToString(), Name = "Folk" }
            );
            ctx.SaveChanges();
            var genreList = ctx.Genres.ToList();

            ctx.Artists.AddRange(new Artist
            {
                ArtistId = Guid.NewGuid().ToString(),
                Name = "Rock Metalson",
                ArtistImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 75,

                Genres = new List<Genre>
                {
                    genreList.First(g => g.Name == "Rock"),
                    genreList.First(g => g.Name == "Metal")
                }

            },

            new Artist
            {
                ArtistId = Guid.NewGuid().ToString(),
                Name = "Pop Starlet",
                ArtistImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 85,
                Genres = new List<Genre>
                {
                    genreList.First(g => g.Name == "Pop"),
                    genreList.First(g => g.Name == "Dance")
                }
            },

            new Artist
            {
                ArtistId = Guid.NewGuid().ToString(),
                Name = "Jazz Maestro",
                ArtistImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 65,
                Genres = new List<Genre>
                {
                    genreList.First(g => g.Name == "Jazz"),
                    genreList.First(g => g.Name == "Blues")
                }
            },

            new Artist
            {
                ArtistId = Guid.NewGuid().ToString(),
                Name = "Classical Virtuoso",
                ArtistImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 50,
                Genres = new List<Genre>
                {
                    genreList.First(g => g.Name == "Classical"),
                    genreList.First(g => g.Name == "Instrumental")
                }
            },

            new Artist
            {
                ArtistId = Guid.NewGuid().ToString(),
                Name = "Hip-Hop Icon",
                ArtistImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 90,
                Genres = new List<Genre>
                {
                    genreList.First(g => g.Name == "Hip-Hop"),
                    genreList.First(g => g.Name == "Rap")
                }
            },

            new Artist
            {
                ArtistId = Guid.NewGuid().ToString(),
                Name = "Electronic Beats",
                ArtistImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 80,
                Genres = new List<Genre>
                {
                    genreList.First(g => g.Name == "Electronic"),
                    genreList.First(g => g.Name == "Dance")
                }
            },

            new Artist
            {
                ArtistId = Guid.NewGuid().ToString(),
                Name = "Country Roads",
                ArtistImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 70,
                Genres = new List<Genre>
                {
                    genreList.First(g => g.Name == "Country"),
                    genreList.First(g => g.Name == "Folk")
                }
            });

            ctx.SaveChanges();
            var artistList = ctx.Artists.ToList();

            ctx.Songs.AddRange(new Song
            {
                SongId = Guid.NewGuid().ToString(),
                Title = "Rock Anthem",
                AlbumImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 78,
                Artists = new List<Artist> { artistList.First(a => a.Name == "Rock Metalson") }
            },

            new Song
            {
                SongId = Guid.NewGuid().ToString(),
                Title = "Pop Hit",
                AlbumImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 88,
                Artists = new List<Artist> { artistList.First(a => a.Name == "Pop Starlet") }
            },

            new Song
            {
                SongId = Guid.NewGuid().ToString(),
                Title = "Jazz Improvisation",
                AlbumImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 68,
                Artists = new List<Artist> { artistList.First(a => a.Name == "Jazz Maestro") }
            },

            new Song
            {
                SongId = Guid.NewGuid().ToString(),
                Title = "Classical Symphony",
                AlbumImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 55,
                Artists = new List<Artist> { artistList.First(a => a.Name == "Classical Virtuoso") }
            },

            new Song
            {
                SongId = Guid.NewGuid().ToString(),
                Title = "Hip-Hop Groove",
                AlbumImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 92,
                Artists = new List<Artist> { artistList.First(a => a.Name == "Hip-Hop Icon") }
            },

            new Song
            {
                SongId = Guid.NewGuid().ToString(),
                Title = "Electronic Vibes",
                AlbumImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 82,
                Artists = new List<Artist> { artistList.First(a => a.Name == "Electronic Beats") }
            },

            new Song
            {
                SongId = Guid.NewGuid().ToString(),
                Title = "Country Ballad",
                AlbumImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 72,
                Artists = new List<Artist> { artistList.First(a => a.Name == "Country Roads") }
            });
            ctx.SaveChanges();
            var songList = ctx.Songs.Include(s => s.Artists).ToList();

            ctx.Users.AddRange(new User
            {
                Id = "1",
                UserName = "user1",
                SpotifyUserId = "spotify_user_1",
                CountryCode = "US",
                ProfilePictureUrl = "",
                Biography = "I love rock and pop music!",
                IsSynthetic = true,
                FavoriteSongs = new List<Song> { songList.First(s => s.Title == "Rock Anthem"), songList.First(s => s.Title == "Pop Hit") },
                FavoriteArtists = new List<Artist> { artistList.First(a => a.Name == "Rock Metalson"), artistList.First(a => a.Name == "Pop Starlet") },
                FavoriteGenres = new List<Genre> { genreList.First(g => g.Name == "Rock"), genreList.First(g => g.Name == "Pop") }
            },

            new User
            {
                Id = "2",
                UserName = "user2",
                SpotifyUserId = "spotify_user_2",
                CountryCode = "GB",
                ProfilePictureUrl = "",
                Biography = "Jazz and classical are my favorites.",
                IsSynthetic = false,
                FavoriteSongs = new List<Song> { songList.First(s => s.Title == "Jazz Improvisation"), songList.First(s => s.Title == "Classical Symphony") },
                FavoriteArtists = new List<Artist> { artistList.First(a => a.Name == "Jazz Maestro"), artistList.First(a => a.Name == "Classical Virtuoso") },
                FavoriteGenres = new List<Genre> { genreList.First(g => g.Name == "Jazz"), genreList.First(g => g.Name == "Classical") }
            },

            new User
            {
                Id = "3",
                UserName = "user3",
                SpotifyUserId = "spotify_user_3",
                CountryCode = "CA",
                ProfilePictureUrl = "",
                Biography = "Can't get enough of hip-hop and electronic music!",
                IsSynthetic = true,
                FavoriteSongs = new List<Song> { songList.First(s => s.Title == "Hip-Hop Groove"), songList.First(s => s.Title == "Electronic Vibes") },
                FavoriteArtists = new List<Artist> { artistList.First(a => a.Name == "Hip-Hop Icon"), artistList.First(a => a.Name == "Electronic Beats") },
                FavoriteGenres = new List<Genre> { genreList.First(g => g.Name == "Hip-Hop"), genreList.First(g => g.Name == "Electronic") }
            },

            new User
            {
                Id = "4",
                UserName = "user4",
                SpotifyUserId = "spotify_user_4",
                CountryCode = "AU",
                ProfilePictureUrl = "",
                Biography = "I enjoy country and folk tunes.",
                IsSynthetic = false,
                FavoriteSongs = new List<Song> { songList.First(s => s.Title == "Country Ballad") },
                FavoriteArtists = new List<Artist> { artistList.First(a => a.Name == "Country Roads") },
                FavoriteGenres = new List<Genre> { genreList.First(g => g.Name == "Country"), genreList.First(g => g.Name == "Folk") }
            },

            new User
            {
                Id = "5",
                UserName = "user5",
                SpotifyUserId = "spotify_user_5",
                CountryCode = "SE",
                ProfilePictureUrl = "",
                Biography = "A mix of everything, really.",
                IsSynthetic = true,
                FavoriteSongs = new List<Song> { songList.First(s => s.Title == "Rock Anthem"), songList.First(s => s.Title == "Jazz Improvisation"), songList.First(s => s.Title == "Hip-Hop Groove") },
                FavoriteArtists = new List<Artist> { artistList.First(a => a.Name == "Rock Metalson"), artistList.First(a => a.Name == "Jazz Maestro"), artistList.First(a => a.Name == "Hip-Hop Icon") },
                FavoriteGenres = new List<Genre> { genreList.First(g => g.Name == "Rock"), genreList.First(g => g.Name == "Jazz"), genreList.First(g => g.Name == "Hip-Hop") }
            },

            new User
            {
                Id = "6",
                UserName = "user6",
                SpotifyUserId = "spotify_user_6",
                CountryCode = "DE",
                ProfilePictureUrl = "",
                Biography = "Electronic and dance music keeps me moving!",
                IsSynthetic = false,
                FavoriteSongs = new List<Song> { songList.First(s => s.Title == "Electronic Vibes"), songList.First(s => s.Title == "Pop Hit") },
                FavoriteArtists = new List<Artist> { artistList.First(a => a.Name == "Electronic Beats"), artistList.First(a => a.Name == "Pop Starlet") },
                FavoriteGenres = new List<Genre> { genreList.First(g => g.Name == "Electronic"), genreList.First(g => g.Name == "Dance") }
            },

            new User
            {
                Id = "7",
                UserName = "user7",
                SpotifyUserId = "spotify_user_7",
                CountryCode = "FR",
                ProfilePictureUrl = "",
                Biography = "Classical music soothes my soul.",
                IsSynthetic = true,
                FavoriteSongs = new List<Song> { songList.First(s => s.Title == "Classical Symphony") },
                FavoriteArtists = new List<Artist> { artistList.First(a => a.Name == "Classical Virtuoso") },
                FavoriteGenres = new List<Genre> { genreList.First(g => g.Name == "Classical"), genreList.First(g => g.Name == "Instrumental") }
            });

            ctx.SaveChanges();
        }
        public void Dispose()
        {
            ctx.Database.EnsureDeleted();
        }

        [Fact]
        public async void GetUser_ShouldReturnUserWithDetails()
        {
            // Arrange
            User? user = null;

            // Act
            user = await userRepository.GetUserWithDetailsAsync("1");

            // Assert - basic checks
            Assert.NotNull(user);
            Assert.Equal("user1", user!.UserName);
            Assert.True(user.IsSynthetic);

            // Assert - favorites count
            Assert.Equal(2, user.FavoriteSongs.Count);
            Assert.Equal(2, user.FavoriteArtists.Count);
            Assert.Equal(2, user.FavoriteGenres.Count);

            // Assert - genres
            Assert.Contains(user.FavoriteGenres, g => g.Name == "Rock");
            Assert.Contains(user.FavoriteGenres, g => g.Name == "Pop");
            Assert.DoesNotContain(user.FavoriteGenres, g => g.Name == "Jazz");

            // Assert - songs
            Assert.Contains(user.FavoriteSongs, s => s.Title == "Pop Hit");
            Assert.DoesNotContain(user.FavoriteSongs, s => s.Title == "Jazz Improvisation");
            Assert.Equal(78, user.FavoriteSongs.First(s => s.Title == "Rock Anthem").Popularity);

            // Assert - artists
            Assert.Equal(75, user.FavoriteArtists.First(a => a.Name == "Rock Metalson").Popularity);
            Assert.Contains(user.FavoriteArtists.First(a => a.Name == "Rock Metalson").Genres, g => g.Name == "Rock");
            Assert.DoesNotContain(user.FavoriteArtists, a => a.Name == "Hip-hop Icon");

            // Assert - bio
            Assert.Contains("rock and pop", user.Biography);
        }

        [Fact]
        public async void GetUser_ShouldReturnNullForNonExistentUser()
        {
            // Arrange
            User? user = null;
            // Act
            user = await userRepository.GetUserWithDetailsAsync("non_existent_id");
            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async void GetUser_ShouldReturnUserWithNoFavorites()
        {
            // Arrange
            var newUser = new User
            {
                Id = "8",
                UserName = "user8",
                SpotifyUserId = "spotify_user_8",
                CountryCode = "IT",
                ProfilePictureUrl = "",
                Biography = "New user with no favorites.",
                IsSynthetic = false
            };
            ctx.Users.Add(newUser);
            ctx.SaveChanges();
            User? user = null;
            // Act
            user = await userRepository.GetUserWithDetailsAsync("8");
            // Assert
            Assert.NotNull(user);
            Assert.Equal("user8", user!.UserName);
            Assert.Empty(user.FavoriteSongs);
            Assert.Empty(user.FavoriteArtists);
            Assert.Empty(user.FavoriteGenres);
        }

        [Fact]
        public async void GetAllUsersWithDetails_ShouldReturnAllUsersWithDetails()
        {
            // Arrange
            IEnumerable<User?> users;
            // Act
            users = await userRepository.GetAllUsersWithDetailsAsync();
            // Assert
            Assert.NotNull(users);
            Assert.Equal(7, users.Count()); // 7 seeded users
            Assert.True(users.All(u => u != null));
            Assert.All(users, u => Assert.NotEmpty(u!.FavoriteSongs));
            Assert.All(users, u => Assert.NotEmpty(u!.FavoriteArtists));
            Assert.All(users, u => Assert.NotEmpty(u!.FavoriteGenres));
            Assert.Contains(users, u => u!.UserName == "user3" && u.FavoriteGenres.Any(g => g.Name == "Hip-Hop"));

        }

        [Fact]
        public async void GetAllUsersWithDetails_ShouldReturnEmptyListWhenNoUsers()
        {
            // Arrange
            ctx.Users.RemoveRange(ctx.Users);
            ctx.SaveChanges();
            IEnumerable<User?> users;
            // Act
            users = await userRepository.GetAllUsersWithDetailsAsync();
            // Assert
            Assert.NotNull(users);
            Assert.Empty(users);
        }

        [Fact]
        public async void DeleteUser_ShouldRemoveUserAndNotAffectSongsArtistsGenres()
        {
            // Arrange
            var userIdToDelete = "5";
            var initialUserCount = ctx.Users.Count();
            var initialSongCount = ctx.Songs.Count();
            var initialArtistCount = ctx.Artists.Count();
            var initialGenreCount = ctx.Genres.Count();

            // Act
            var userToDelete = await userRepository.GetByIdAsync(userIdToDelete);
            Assert.NotNull(userToDelete); // Ensure user exists before deletion
            await userRepository.DeleteAsync(userIdToDelete);
            await userRepository.SaveChangesAsync();
            var postDeleteUserCount = ctx.Users.Count();
            var postDeleteSongCount = ctx.Songs.Count();
            var postDeleteArtistCount = ctx.Artists.Count();
            var postDeleteGenreCount = ctx.Genres.Count();
            var deletedUser = await userRepository.GetByIdAsync(userIdToDelete);

            // Assert
            Assert.Null(deletedUser); // User should be deleted
            Assert.Equal(initialUserCount - 1, postDeleteUserCount); // User count should decrease by 1
            Assert.Equal(initialSongCount, postDeleteSongCount); // Song count should remain the same
            Assert.Equal(initialArtistCount, postDeleteArtistCount); // Artist count should remain the same
            Assert.Equal(initialGenreCount, postDeleteGenreCount); // Genre count should remain the same
            // Additionally, ensure no songs, artists, or genres are orphaned (not strictly necessary in this unidirectional setup)

            foreach (var song in ctx.Songs.Include(s => s.Artists))
            {
                Assert.NotEmpty(song.Artists); // Each song should still have its artists
            }
            foreach (var artist in ctx.Artists.Include(a => a.Genres))
            {
                Assert.NotEmpty(artist.Genres); // Each artist should still have its genres
            }
        }

        [Fact]
        public async void DeleteUser_ShouldHandleNonExistentUser()
        {
            // Arrange
            var nonExistentUserId = "non_existent_id";
            var initialUserCount = ctx.Users.Count();

            // Act
            await userRepository.DeleteAsync(nonExistentUserId);
            var postDeleteUserCount = ctx.Users.Count();

            // Assert
            Assert.Equal(initialUserCount, postDeleteUserCount); // User count should remain the same
        }

        [Fact]
        public async void AddUser_ShouldAddNewUser()
        {
            // Arrange
            var newUser = new User
            {
                Id = "9",
                UserName = "user9",
                SpotifyUserId = "spotify_user_9",
                CountryCode = "NL",
                ProfilePictureUrl = "",
                Biography = "Excited to join SoundMatch!",
                IsSynthetic = false
            };
            var initialUserCount = ctx.Users.Count();
            // Act
            await userRepository.AddAsync(newUser);
            await userRepository.SaveChangesAsync();
            var postAddUserCount = ctx.Users.Count();
            var addedUser = await userRepository.GetByIdAsync("9");
            // Assert
            Assert.NotNull(addedUser);
            Assert.Equal("user9", addedUser!.UserName);
            Assert.Equal(initialUserCount + 1, postAddUserCount); // User count should increase by 1
        }

        [Fact]
        public async void AddUser_ShouldAddSongsArtistsGenres()
        {
            // Arrange
            var genre = new Genre { GenreId = Guid.NewGuid().ToString(), Name = "NewGenre" };
            var artist = new Artist
            {
                ArtistId = Guid.NewGuid().ToString(),
                Name = "NewArtist",
                ArtistImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 60,
                Genres = new List<Genre> { genre }
            };
            var song = new Song
            {
                SongId = Guid.NewGuid().ToString(),
                Title = "NewSong",
                AlbumImageUrl = "",
                SpotifyId = Guid.NewGuid().ToString(),
                Popularity = 70,
                Artists = new List<Artist> { artist }
            };
            var newUser = new User
            {
                Id = "10",
                UserName = "user10",
                SpotifyUserId = "spotify_user_10",
                CountryCode = "BE",
                ProfilePictureUrl = "",
                Biography = "Music lover.",
                IsSynthetic = false,
                FavoriteSongs = new List<Song> { song },
                FavoriteArtists = new List<Artist> { artist },
                FavoriteGenres = new List<Genre> { genre }
            };
            var initialUserCount = ctx.Users.Count();
            var initialSongCount = ctx.Songs.Count();
            var initialArtistCount = ctx.Artists.Count();
            var initialGenreCount = ctx.Genres.Count();
            // Act
            await userRepository.AddAsync(newUser);
            await userRepository.SaveChangesAsync();
            var postAddUserCount = ctx.Users.Count();
            var postAddSongCount = ctx.Songs.Count();
            var postAddArtistCount = ctx.Artists.Count();
            var postAddGenreCount = ctx.Genres.Count();
            var addedUser = await userRepository.GetByIdAsync("10");
            // Assert
            Assert.NotNull(addedUser);
            Assert.Equal("user10", addedUser!.UserName);
            Assert.Equal(initialUserCount + 1, postAddUserCount); // User count should increase by 1
            Assert.Equal(initialSongCount + 1, postAddSongCount); // Song count should increase by 1
            Assert.Equal(initialArtistCount + 1, postAddArtistCount); // Artist count should increase by 1
            Assert.Equal(initialGenreCount + 1, postAddGenreCount); // Genre count should increase by

        }
    }
}