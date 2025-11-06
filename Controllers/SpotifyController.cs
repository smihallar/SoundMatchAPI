using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;
using System.Net;

namespace SoundMatchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyDataService spotifyDataService;
        private readonly ISpotifyAuthService spotifyAuthService;
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;

        public SpotifyController(ISpotifyDataService spotifyDataService, ISpotifyAuthService spotifyAuthService, UserManager<User> userManager, IConfiguration configuration)
        {
            this.spotifyDataService = spotifyDataService;
            this.spotifyAuthService = spotifyAuthService;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        // GET: api/Spotify/login
        [HttpGet("login")]
        public async Task<ActionResult<ReturnResponse<SpotifyAuthorizationUrlResponse>>> Login()
        {
            var returnResponse = await spotifyAuthService.GetAuthorizationUrl();
            if (returnResponse.Data == null || string.IsNullOrEmpty(returnResponse.Data.AuthorizationUrl))
            {
                return new ReturnResponse<SpotifyAuthorizationUrlResponse>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Failed to get authorization URL.",
                    Data = null
                };
            }
            return new ReturnResponse<SpotifyAuthorizationUrlResponse>
            {
                StatusCode = returnResponse.StatusCode,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>(),
                Data = returnResponse.Data ?? null
            };
        }

        // GET: api/Spotify/callback
        [HttpGet("callback")]
        public async Task<ActionResult<ReturnResponse>> Callback([FromQuery] string code, [FromQuery] string state)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return new ReturnResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = "Log in to access this resource.",
                    Errors = new List<string> { "User is not logged in." }
                };
            }
            var tokenReturnResponse = await spotifyAuthService.ExchangeCodeAndStoreTokensAsync(user, code);
            if (tokenReturnResponse.StatusCode != HttpStatusCode.OK || tokenReturnResponse.Data == null || string.IsNullOrEmpty(tokenReturnResponse.Data.AccessToken))
            {
                return new ReturnResponse
                {
                    StatusCode = tokenReturnResponse.StatusCode,
                    Message = tokenReturnResponse.Message,
                    Errors = tokenReturnResponse.Errors
                };
            }
            var spotifyDataReturnResponse = await spotifyDataService.ConnectSpotifyAndPopulateMusicAsync(user, tokenReturnResponse.Data.AccessToken);

            if (spotifyDataReturnResponse.StatusCode == HttpStatusCode.OK)
            {
                return Redirect($"{configuration["ClientUrl"]}/user-profile/{user.Id}");
            }

            return new ReturnResponse
            {
                StatusCode = spotifyDataReturnResponse.StatusCode,
                Message = spotifyDataReturnResponse.Message,
                Errors = spotifyDataReturnResponse.Errors
            };
        }

        // POST: api/Spotify/refresh-top-items
        [HttpPost("refresh-top-items")]
        public async Task<ActionResult<ReturnResponse<UserProfileResponse>>> RefreshTopItems()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = "Log in to access this resource.",
                    Errors = new List<string> { "User is not logged in." },
                    Data = null
                };
            }
            var tokenReturnResponse = await spotifyAuthService.GetAccessTokenAsync(user);
            if (tokenReturnResponse.StatusCode != HttpStatusCode.OK || tokenReturnResponse.Data == null || string.IsNullOrEmpty(tokenReturnResponse.Data.AccessToken))
            {
                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = tokenReturnResponse.StatusCode,
                    Message = tokenReturnResponse.Message,
                    Errors = tokenReturnResponse.Errors,
                    Data = null
                };
            }

            var returnResponse = await spotifyDataService.RefreshTopItemsAsync(user, tokenReturnResponse.Data.AccessToken);

            return new ReturnResponse<UserProfileResponse>
            {
                StatusCode = returnResponse.StatusCode,
                Data = returnResponse.Data ?? null,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>()
            };
        }

        // POST: api/Spotify/refresh-profile
        [HttpPost("refresh-profile")]
        [Authorize]
        public async Task<ActionResult<ReturnResponse<UserProfileResponse>>> RefreshProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = "Log in to access this resource.",
                    Errors = new List<string> { "User is not logged in." },
                    Data = null
                };
            }

            var tokenReturnResponse = await spotifyAuthService.GetAccessTokenAsync(user);
            if (tokenReturnResponse.StatusCode != HttpStatusCode.OK || tokenReturnResponse.Data == null || string.IsNullOrEmpty(tokenReturnResponse.Data.AccessToken))
            {
                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = tokenReturnResponse.StatusCode,
                    Message = tokenReturnResponse.Message,
                    Errors = tokenReturnResponse.Errors,
                    Data = null
                };
            }

            var returnResponse = await spotifyDataService.RefreshUserProfileAsync(user, tokenReturnResponse.Data.AccessToken);

            return new ReturnResponse<UserProfileResponse>
            {
                StatusCode = returnResponse.StatusCode,
                Data = returnResponse.Data ?? null,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>()
            };
        }
    }
}
