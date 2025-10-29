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
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    if (returnResponse.Data == null || string.IsNullOrEmpty(returnResponse.Data.AuthorizationUrl))
                        return StatusCode(StatusCodes.Status500InternalServerError, "Authorization URL is missing.");
                    return Ok(returnResponse.Data);
            }
        }

        // GET: api/Spotify/callback
        [HttpGet("callback")]
        public async Task<ActionResult<ReturnResponse>> Callback([FromQuery] string code, [FromQuery] string state)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }
            var tokenReturnResponse = await spotifyAuthService.ExchangeCodeAndStoreTokensAsync(user, code);
            if (tokenReturnResponse.StatusCode != HttpStatusCode.OK || tokenReturnResponse.Data == null || string.IsNullOrEmpty(tokenReturnResponse.Data.AccessToken))
                return StatusCode((int)tokenReturnResponse.StatusCode, tokenReturnResponse);

            var spotifyDataReturnResponse = await spotifyDataService.ConnectSpotifyAndPopulateMusicAsync(user, tokenReturnResponse.Data.AccessToken);

            switch (spotifyDataReturnResponse.StatusCode)
            {
                case HttpStatusCode.Forbidden:
                    return Forbid();
                case HttpStatusCode.NotFound:
                    return NotFound(spotifyDataReturnResponse);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, spotifyDataReturnResponse);
                default:
                    return Redirect($"{configuration["ClientUrl"]}/profile/{user.Id}");
            }
        }

        // POST: api/Spotify/refresh-top-items
        [HttpPost("refresh-top-items")]
        [Authorize]
        public async Task<ActionResult<ReturnResponse<UserProfileResponse>>> RefreshTopItems()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Forbid();

            var tokenReturnResponse = await spotifyAuthService.GetAccessTokenAsync(user);
            if (tokenReturnResponse.StatusCode != HttpStatusCode.OK || tokenReturnResponse.Data == null || string.IsNullOrEmpty(tokenReturnResponse.Data.AccessToken))
                return StatusCode((int)tokenReturnResponse.StatusCode, tokenReturnResponse);

            var returnResponse = await spotifyDataService.RefreshTopItemsAsync(user, tokenReturnResponse.Data.AccessToken);

            switch(returnResponse.StatusCode)
            {
                case HttpStatusCode.Forbidden:
                    return Forbid();
                case HttpStatusCode.NotFound:
                    return NotFound(returnResponse);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse.Data);
            }
        }

        // POST: api/Spotify/refresh-profile
        [HttpPost("refresh-profile")]
        [Authorize]
        public async Task<ActionResult<ReturnResponse<UserProfileResponse>>> RefreshProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Forbid();

            var tokenReturnResponse = await spotifyAuthService.GetAccessTokenAsync(user);
            if (tokenReturnResponse.StatusCode != HttpStatusCode.OK || tokenReturnResponse.Data == null || string.IsNullOrEmpty(tokenReturnResponse.Data.AccessToken))
                return StatusCode((int)tokenReturnResponse.StatusCode, tokenReturnResponse);

            var returnResponse = await spotifyDataService.RefreshUserProfileAsync(user, tokenReturnResponse.Data.AccessToken);

            switch(returnResponse.StatusCode)
            {
                case HttpStatusCode.Forbidden:
                    return Forbid();
                case HttpStatusCode.NotFound:
                    return NotFound(returnResponse);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse.Data);
            }
        }
    }
}
