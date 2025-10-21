using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Data.DTOs.Requests;
using SoundMatchAPI.Services;

namespace SoundMatchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService authService;

        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var result = await authService.RegisterUserAsync(request);
        }
    }
}
