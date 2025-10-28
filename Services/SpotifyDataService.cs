using AutoMapper;
using SoundMatchAPI.Data.DomainModels;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;
using System.Net;
using System.Text.Json;

namespace SoundMatchAPI.Services
{
    public class SpotifyDataService : ISpotifyDataService
    {
        private readonly HttpClient httpClient;
        private readonly IMusicService musicService;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public SpotifyDataService(HttpClient httpClient, IMusicService musicService, IUserRepository userRepository, IMapper mapper)
        {
            this.httpClient = httpClient;
            this.musicService = musicService;
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<ReturnResponse<UserProfileResponse>> ConnectSpotifyAndPopulateMusicAsync(User user, string accessToken)
        {
            try
            {
                // Fetch user profile
                var profileResponse = await RefreshUserProfileAsync(user, accessToken);
                if (profileResponse.StatusCode != HttpStatusCode.OK) return profileResponse;

                // Fetch top items
                var topItemsResponse = await RefreshTopItemsAsync(user, accessToken);
                if (topItemsResponse.StatusCode != HttpStatusCode.OK) return topItemsResponse;

                // Map final user profile
                var userProfile = mapper.Map<UserProfileResponse>(user);

                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = userProfile,
                    Message = "Spotify connected and fetched data successfully."
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Failed to fetch Spotify data.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ReturnResponse<UserProfileResponse>> RefreshTopItemsAsync(User user, string accessToken)
        {
            try
            {
                var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
                if (user.MusicTasteLastRefreshed > oneWeekAgo) // Already refreshed within the last week
                {
                    return new ReturnResponse<UserProfileResponse>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Data = mapper.Map<UserProfileResponse>(user),
                        Message = "User top items refreshed recently. You can refresh music taste once per week"
                    };
                }
                // Fetch top 50 artists, long term (1 year)
                var artistsRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/top/artists?time_range=long_term&limit=50&offset=0");
                artistsRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var artistsResponse = await httpClient.SendAsync(artistsRequest);
                artistsResponse.EnsureSuccessStatusCode();

                var artistsJson = await artistsResponse.Content.ReadAsStringAsync();
                var topArtists = JsonSerializer.Deserialize<SpotifyTopArtistsResponse>(artistsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Fetch top 50 tracks, long term (1 year)
                var tracksRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/top/tracks?time_range=long_term&limit=50&offset=0");
                tracksRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var tracksResponse = await httpClient.SendAsync(tracksRequest);
                tracksResponse.EnsureSuccessStatusCode();

                var tracksJson = await tracksResponse.Content.ReadAsStringAsync();
                var topTracks = JsonSerializer.Deserialize<SpotifyTopTracksResponse>(tracksJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (topArtists == null || topTracks == null)
                {
                    return new ReturnResponse<UserProfileResponse>
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Message = "Error while fetching music data",
                        Errors = new List<string> { "Top items data is null." }
                    };
                }
                // Map spotify responses to models
                var artists = mapper.Map<List<Artist>>(topArtists.Items);
                var tracks = mapper.Map<List<Song>>(topTracks.Items);

                // Collect genres from artists
                var genres = artists.SelectMany(a => a.Genres).GroupBy(g => g.Name).Select(g => g.First()).ToList();

                var musicProfile = new MusicProfile(tracks, artists, genres);
                await musicService.SaveSpotifyMusicAsync(musicProfile);

                // Update user entity
                user.FavoriteArtists.AddRange(artists);
                user.FavoriteArtistIds.AddRange(artists.Select(a => a.ArtistId));
                user.FavoriteSongs.AddRange(tracks);
                user.FavoriteSongIds.AddRange(tracks.Select(s => s.SongId));
                user.FavoriteGenres.AddRange(genres);
                user.FavoriteGenreIds.AddRange(genres.Select(g => g.GenreId));
                user.MusicTasteLastRefreshed = DateTime.UtcNow;

                await userRepository.UpdateAsync(user);

                var userProfile = mapper.Map<UserProfileResponse>(user);

                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = userProfile,
                    Message = "User top items refreshed successfully."
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Failed to refresh user top items.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ReturnResponse<UserProfileResponse>> RefreshUserProfileAsync(User user, string accessToken)
        {
            try
            {
                var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
                if (user.UserDetailsLastRefreshed > oneWeekAgo) // Already refreshed within the last week
                {
                    return new ReturnResponse<UserProfileResponse>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Data = mapper.Map<UserProfileResponse>(user),
                        Message = "User profile refreshed recently. You can refresh user details once per week"
                    };
                }
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var spotifyUserJson = await response.Content.ReadAsStringAsync();
                var spotifyUser = JsonSerializer.Deserialize<SpotifyUserResponse>(spotifyUserJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (spotifyUser == null) throw new Exception("Failed to fetch Spotify user profile.");

                // Update user entity
                user.SpotifyUserId = spotifyUser.Id;
                user.CountryCode = spotifyUser.Country;
                user.ProfilePictureUrl = spotifyUser.Images?.FirstOrDefault()?.Url ?? string.Empty;
                user.UserDetailsLastRefreshed = DateTime.UtcNow;

                await userRepository.UpdateAsync(user);

                var userProfile = mapper.Map<UserProfileResponse>(user);

                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = userProfile,
                    Message = "User profile refreshed successfully."
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Failed to refresh user profile.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
