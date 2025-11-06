using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SoundMatchAPI.Data.AuthModels;
using SoundMatchAPI.Data.DTOs.Requests;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Services;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoundMatchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        //POST: api/Auth/register
        [HttpPost("register")]
        public async Task<ActionResult<ReturnResponse>> Register([FromBody] UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new ReturnResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid request.",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                };
            }
            var returnResponse = await authService.RegisterUserAsync(request);

            return new ReturnResponse<AuthResponse>
            {
                StatusCode = returnResponse.StatusCode,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>(),
            };
        }

        //POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<ReturnResponse<AuthResponse>>> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new ReturnResponse<AuthResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid request.",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList(),
                    Data = null
                };
            }
            var returnResponse = await authService.LoginUserAsync(request);

            return new ReturnResponse<AuthResponse>
            {
                StatusCode = returnResponse.StatusCode,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>(),
                Data = returnResponse.Data ?? null
            };
        }
    }
}
