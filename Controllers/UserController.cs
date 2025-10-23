using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Constants;
using SoundMatchAPI.Data.DTOs.Requests;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Services;
using System.Net;

namespace SoundMatchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var returnResponse = await userService.GetAllUsersAsync();
            switch(returnResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse);
            }
        }
        // GET: api/User/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var returnResponse = await userService.GetUserByIdAsync(userId);
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse);
            }
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var uId = User.FindFirst(CustomClaimTypes.Uid)?.Value; // Id of user that is logged in
            if (uId == null)
            {
                return Forbid();
            }
            var returnResponse = await userService.DeleteUserAsync(userId, uId); // 
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.Forbidden:
                    return StatusCode(StatusCodes.Status403Forbidden, returnResponse);
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse);
            }
        }

        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            var returnResponse = await userService.GetUserProfileAsync(userId);
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse);
            }
        }
    }
}
