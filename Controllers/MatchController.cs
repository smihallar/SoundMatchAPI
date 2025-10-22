using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Constants;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using System.Net;

namespace SoundMatchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService matchService;

        public MatchController(IMatchService matchService)
        {
            this.matchService = matchService;
        }

        // GET: api/Match/{matchId}
        [HttpGet("{matchId}")]
        [Authorize]
        public async Task<IActionResult> GetMatch(string matchId)
        {
            var loggedInUserId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
            var returnResponse = await matchService.GetMatchByIdWithDetailsAsync(matchId, loggedInUserId);
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return NotFound(returnResponse);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse.Data);
            }
        }

        // Find and create new matches for a user
        // POST: api/Match/all/{userId}
        [HttpPost("all/{userId}")]
        public async Task<IActionResult> FindMatches(string userId)
        {
            var loggedInUserId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
            if (loggedInUserId == null)
            {
                return Forbid();
            }
            var returnResponse = await matchService.AddMatches(userId, loggedInUserId);
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return NotFound(returnResponse);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse.Data);
            }
        }

        // GET: api/Match/all/{userId}   
        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetAllMatches(string userId)
        {
            var loggedInUserId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
            if (loggedInUserId == null)
            {
                return Forbid();
            }
            var returnResponse = await matchService.GetMatchesByUserIdAsync(userId, loggedInUserId);
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return NotFound(returnResponse);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse.Data);
            }
        }

        [HttpDelete("{matchId}")]
        public async Task<IActionResult> DeleteMatch(string matchId)
        {
            var loggedInUserId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
            if (loggedInUserId == null)
            {
                return Forbid();
            }
            var returnResponse = await matchService.DeleteMatchAsync(matchId, loggedInUserId);
            switch (returnResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return NotFound(returnResponse);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                default:
                    return Ok(returnResponse.Message);
            }
        }
    }
}
