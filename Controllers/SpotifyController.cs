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
        public IActionResult Login()
        {
            var authorizationUrl = spotifyAuthService.GetAuthorizationUrl();
            return Redirect(authorizationUrl);
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

            var returnResponse = await spotifyDataService.ConnectSpotifyAndPopulateMusicAsync(user, code);

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
            };
        }
    }
}
