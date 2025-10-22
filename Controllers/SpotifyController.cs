using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Data.Interfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;

namespace SoundMatchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        public SpotifyController(ISpotifyService spotifyService)
        {
            
        }
    }
}
