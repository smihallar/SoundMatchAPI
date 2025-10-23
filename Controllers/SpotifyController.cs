using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Data.Interfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyDataService spotifyDataService;
        public SpotifyController(ISpotifyDataService spotifyDataService)
        {
            this.spotifyDataService = spotifyDataService;
        }

        // GET: api/Spotify/callback
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code, string state)
        {
            // Handle Spotify OAuth callback
            return Ok();
        }
    }
}
