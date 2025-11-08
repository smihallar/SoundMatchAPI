using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;
using SoundMatchAPI.Data.Repositories;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace SoundMatchAPI.Services
{
    public class SpotifyAuthService : ISpotifyAuthService
    {
        private readonly HttpClient httpClient;
        private readonly IUserRepository userRepository;
        private readonly IConfiguration config;
        public SpotifyAuthService(HttpClient httpClient, IUserRepository userRepository, IConfiguration config)
        {
            this.httpClient = httpClient;
            this.userRepository = userRepository;
            this.config = config;
        }

        public async Task<ReturnResponse<SpotifyTokenResponse>> ExchangeCodeAndStoreTokensAsync(User user, string code)
        {
            try
            {
                if(user.IsConnectedToSpotify)
                {
                    return new ReturnResponse<SpotifyTokenResponse>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "User is already connected to Spotify.",
                        Errors = new List<string> { "Spotify account is already linked." }
                    };
                }
                var clientId = config["Spotify:ClientId"];
                var clientSecret = config["Spotify:ClientSecret"];
                var redirectUri = config["Spotify:RedirectUri"];

                // 1️⃣ Prepare the POST request to Spotify
                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["grant_type"] = "authorization_code",
                        ["code"] = code,
                        ["redirect_uri"] = redirectUri!,
                        ["client_id"] = clientId!,
                        ["client_secret"] = clientSecret!
                    })
                };

                // 2️⃣ Send the request
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var token = JsonSerializer.Deserialize<SpotifyTokenResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new Exception("Failed to deserialize Spotify token response");


                // 4️⃣ Store tokens and update user
                await userRepository.UpdateUserWithSpotifyAuthDetailsAsync(user.Id, token.RefreshToken!, DateTime.UtcNow.AddSeconds(token.ExpiresIn));

                // 5️⃣ Return the access token in a standardized response
                return new ReturnResponse<SpotifyTokenResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = token,
                    Message = "Spotify tokens exchanged successfully."
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<SpotifyTokenResponse>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Failed to exchange Spotify code.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ReturnResponse<SpotifyTokenResponse>> GetAccessTokenAsync(User user)
        {
            try
            {
                var clientId = config["Spotify:ClientId"];
                var clientSecret = config["Spotify:ClientSecret"];

                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["grant_type"] = "refresh_token",
                        ["refresh_token"] = user.SpotifyRefreshToken!,
                        ["client_id"] = clientId!,
                        ["client_secret"] = clientSecret!
                    })
                };

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<SpotifyTokenResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return new ReturnResponse<SpotifyTokenResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = token
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<SpotifyTokenResponse>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Failed to refresh Spotify token.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }



        // Generates the Spotify authorization URL, including client ID, redirect URI and scopes from configuration
        public async Task<ReturnResponse<SpotifyAuthorizationUrlResponse>> GetAuthorizationUrl(string userId)
        {
            try
            {
                var clientId = config["Spotify:ClientId"] ?? string.Empty;
                var redirectUri = Uri.EscapeDataString(config["Spotify:RedirectUri"] ?? string.Empty);
                var scopes = Uri.EscapeDataString(config["Spotify:Scopes"] ?? string.Empty);
                var state = userId; // user id to keep track of logged in user sending the request

                // Force Spotify to show the login/authorization dialog
                var showDialog = "true";

                var url = $"https://accounts.spotify.com/authorize?response_type=code" +
                          $"&client_id={clientId}" +
                          $"&scope={scopes}" +
                          $"&redirect_uri={redirectUri}" +
                          $"&state={state}" +
                          $"&show_dialog={showDialog}";

                // Add an artificial await to avoid CS1998
                await Task.CompletedTask;

                return new ReturnResponse<SpotifyAuthorizationUrlResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = new SpotifyAuthorizationUrlResponse
                    {
                        AuthorizationUrl = url
                    },
                    Message = "Spotify authorization URL generated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<SpotifyAuthorizationUrlResponse>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Failed to generate Spotify authorization URL.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
