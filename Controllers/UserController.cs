using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Constants;
using SoundMatchAPI.Data.DTOs.Requests;
using SoundMatchAPI.Data.DTOs.Responses;
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

        // GET: api/User/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<ReturnResponse<UserResponse>>> GetUserById(string userId)
        {
            var returnResponse = await userService.GetUserByIdAsync(userId);
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse.Data);
            }
        }

        // PUT: api/User/bio/{userId}
        [HttpPut("bio/{userId}")]
        [Authorize]
        public async Task<ActionResult<ReturnResponse<UserProfileResponse>>> UpdateUserBio(string userId, [FromBody] UpdateUserBioRequest request)
        {
            var uId = User.FindFirst(CustomClaimTypes.Uid)?.Value; // Id of user that is logged in
            if (uId == null)
            {
                return Forbid();
            }
            var returnResponse = await userService.UpdateUserBioAsync(userId, request.Bio, uId);
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.Forbidden:
                    return StatusCode(StatusCodes.Status403Forbidden, returnResponse);
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse.Data);
            }
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<ActionResult<ReturnResponse>> DeleteUser(string userId)
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
        public async Task<ActionResult<ReturnResponse<UserProfileResponse>>> GetUserProfile(string userId)
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
