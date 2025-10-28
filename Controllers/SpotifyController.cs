using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Data.Interfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;
using SoundMatchAPI.Services;
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

        public SpotifyController(ISpotifyDataService spotifyDataService, ISpotifyAuthService spotifyAuthService, UserManager<User> userManager)
        {
            this.spotifyDataService = spotifyDataService;
            this.spotifyAuthService = spotifyAuthService;
            this.userManager = userManager;
        }

        // GET: api/Spotify/login
        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            var returnResponse = await spotifyAuthService.GetAuthorizationUrl();
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    if (string.IsNullOrEmpty(returnResponse.Data))
                        return StatusCode(StatusCodes.Status500InternalServerError, "Authorization URL is missing.");
                    return Ok(returnResponse.Data);
            }
            ;
        }

        // GET: api/Spotify/callback
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }
            var tokenResponse = await spotifyAuthService.ExchangeCodeAndStoreTokensAsync(user, code);
            if (tokenResponse.StatusCode != HttpStatusCode.OK)
                return StatusCode((int)tokenResponse.StatusCode, tokenResponse);
            var returnResponse = await spotifyDataService.ConnectSpotifyAndPopulateMusicAsync(user, tokenResponse.Data!);

            switch (returnResponse.StatusCode)
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
            ;
        }

        // POST: api/Spotify/refresh-top-items
        [HttpPost("refresh-top-items")]
        [Authorize]
        public async Task<IActionResult> RefreshTopItems()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Forbid();

            var tokenResponse = await spotifyAuthService.GetAccessTokenAsync(user);
            if (tokenResponse.StatusCode != HttpStatusCode.OK)
                return StatusCode((int)tokenResponse.StatusCode, tokenResponse);

            var returnResponse = await spotifyDataService.RefreshTopItemsAsync(user, tokenResponse.Data!);

            return returnResponse.StatusCode switch
            {
                HttpStatusCode.Forbidden => Forbid(),
                HttpStatusCode.NotFound => NotFound(returnResponse),
                HttpStatusCode.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, returnResponse),
                _ => Ok(returnResponse.Data)
            };
        }

        // POST: api/Spotify/refresh-profile
        [HttpPost("refresh-profile")]
        [Authorize]
        public async Task<IActionResult> RefreshProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Forbid();

            var tokenResponse = await spotifyAuthService.GetAccessTokenAsync(user);
            if (tokenResponse.StatusCode != HttpStatusCode.OK)
                return StatusCode((int)tokenResponse.StatusCode, tokenResponse);

            var returnResponse = await spotifyDataService.RefreshUserProfileAsync(user, tokenResponse.Data!);

            return returnResponse.StatusCode switch
            {
                HttpStatusCode.Forbidden => Forbid(),
                HttpStatusCode.NotFound => NotFound(returnResponse),
                HttpStatusCode.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, returnResponse),
                _ => Ok(returnResponse.Data)
            };
        }
    }
}
