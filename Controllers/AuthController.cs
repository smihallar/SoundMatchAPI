using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SoundMatchAPI.Data.AuthModels;
using SoundMatchAPI.Data.DTOs.Requests;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Services;

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
        public async Task<ActionResult<ReturnResponse<AuthResponse>>> Register([FromBody] UserRegisterRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var returnResponse = await authService.RegisterUserAsync(request);
            switch(returnResponse.StatusCode)
            {
                case System.Net.HttpStatusCode.BadRequest:
                    if (returnResponse.Errors != null)
                    {
                        foreach (var error in returnResponse.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                    return BadRequest(ModelState);
                case System.Net.HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                case System.Net.HttpStatusCode.OK:
                    return Ok(returnResponse.Data);
            }
        }

        //POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<ReturnResponse<AuthResponse>>> Login([FromBody] UserLoginRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var returnResponse = await authService.LoginUserAsync(request);

            switch(returnResponse.StatusCode)
            {
                case System.Net.HttpStatusCode.BadRequest:
                    if (returnResponse.Errors != null)
                    {
                        foreach (var error in returnResponse.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                    return BadRequest(ModelState);
                case System.Net.HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                case System.Net.HttpStatusCode.OK:
                    return Ok(returnResponse.Data);
            }
        }
    }
}
