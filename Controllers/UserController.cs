using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Constants;
using SoundMatchAPI.Data.DTOs.Requests;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Services;
using SoundMatchAPI.Data.Models;
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
            return new ReturnResponse<UserResponse>
            {
                StatusCode = returnResponse.StatusCode,
                Data = returnResponse.Data ?? null,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>()
            };
        }

        // PUT: api/User/bio/{userId}
        [HttpPut("bio/{userId}")]
        [Authorize]
        public async Task<ActionResult<ReturnResponse<UserProfileResponse>>> UpdateUserBio(string userId, [FromBody] UpdateUserBioRequest request)
        {
            var uId = User.FindFirst(CustomClaimTypes.Uid)?.Value; // Id of user that is logged in
            if (uId == null)
            {
                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = "Log in to access this resource.",
                    Errors = new List<string> { "User is not logged in." },
                    Data = null
                };
            }
            var returnResponse = await userService.UpdateUserBioAsync(userId, request.Bio, uId);
            return new ReturnResponse<UserProfileResponse>
            {
                StatusCode = returnResponse.StatusCode,
                Data = returnResponse.Data ?? null,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>()
            };
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<ActionResult<ReturnResponse>> DeleteUser(string userId)
        {
            var uId = User.FindFirst(CustomClaimTypes.Uid)?.Value; // Id of user that is logged in
            if (uId == null)
            {
                return new ReturnResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = "Log in to access this resource.",
                    Errors = new List<string> { "User is not logged in." }
                };
            }
            var returnResponse = await userService.DeleteUserAsync(userId, uId); // 
            return new ReturnResponse
            {
                StatusCode = returnResponse.StatusCode,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>()
            };
        }

        // GET: api/User/profile/{userId}
        [HttpGet("profile/{userId}")]
        public async Task<ActionResult<ReturnResponse<UserProfileResponse>>> GetUserProfile(string userId)
        {
            var returnResponse = await userService.GetUserProfileAsync(userId);
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
