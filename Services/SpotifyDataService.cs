using AutoMapper;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;
using System.Net;

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
                // Fetch top artists
                var artistsRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/top/artists?limit=20");
                artistsRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var artistsResponse = await httpClient.SendAsync(artistsRequest);
                artistsResponse.EnsureSuccessStatusCode();
                var topArtists = await artistsResponse.Content.ReadFromJsonAsync<SpotifyTopArtistsResponse>();

                // Fetch top tracks
                var tracksRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/top/tracks?limit=20");
                tracksRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var tracksResponse = await httpClient.SendAsync(tracksRequest);
                tracksResponse.EnsureSuccessStatusCode();
                var topTracks = await tracksResponse.Content.ReadFromJsonAsync<SpotifyTopTracksResponse>();

                // Upsert music
                if (topArtists?.Items != null) await musicService.UpsertArtistsAndGenresAsync(topArtists.Items);
                if (topTracks?.Items != null) await musicService.UpsertTracksAsync(topTracks.Items);

                // Update user favorites
                user.FavoriteArtists = topArtists?.Items.Select(a => musicService.GetOrCreateArtist(a)).ToList() ?? new List<Artist>();
                user.FavoriteSongs = topTracks?.Items.Select(t => musicService.GetOrCreateSong(t)).ToList() ?? new List<Song>();
                user.MusicTasteLastRefreshed = DateTime.UtcNow;

                await userRepository.UpdateAsync(user);

                var userProfile = mapper.Map<UserProfileResponse>(user);
                userProfile.FavoriteSongs = user.FavoriteSongs;
                userProfile.FavoriteArtists = user.FavoriteArtists;
                userProfile.FavoriteGenres = user.FavoriteGenres;

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
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var spotifyUser = await response.Content.ReadFromJsonAsync<SpotifyUserResponse>();
                if (spotifyUser == null) throw new Exception("Failed to fetch Spotify user profile.");

                // Update user entity
                user.SpotifyUserId = spotifyUser.Id;
                user.CountryCode = spotifyUser.Country;
                user.ProfilePictureUrl = spotifyUser.Images?.FirstOrDefault()?.Url ?? string.Empty;
                user.Biography = spotifyUser.Bio ?? string.Empty;

                await userRepository.UpdateAsync(user);

                var userProfile = mapper.Map<UserProfileResponse>(user);
                userProfile.FavoriteSongs = user.FavoriteSongs;
                userProfile.FavoriteArtists = user.FavoriteArtists;
                userProfile.FavoriteGenres = user.FavoriteGenres;

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
