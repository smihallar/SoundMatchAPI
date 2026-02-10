using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Constants;
using SoundMatchAPI.Data.DTOs.Responses;
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
        public async Task<ActionResult<ReturnResponse<MatchResponse>>> GetMatch(string matchId)
        {
            var loggedInUserId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
            if (loggedInUserId == null)
            {
                return new ReturnResponse<MatchResponse>
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = "Log in to access this resource.",
                    Data = null,
                    Errors = new List<string> { "User is not logged in." }
                };
            }
            var returnResponse = await matchService.GetMatchByIdWithDetailsAsync(matchId, loggedInUserId);
            return new ReturnResponse<MatchResponse>
            {
                StatusCode = returnResponse.StatusCode,
                Data = returnResponse.Data ?? null,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>()
            };
        }

        // Find and create new matches for a user
        // POST: api/Match/find-matches/{userId}
        [HttpPost("find-matches/{userId}")]
        //[Authorize]
        public async Task<ActionResult<ReturnResponse<List<MatchResponse>>>> FindMatches(string userId)
        {
            var loggedInUserId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
            if (loggedInUserId == null)
            {
                return new ReturnResponse<List<MatchResponse>>
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = "Log in to access this resource.",
                    Data = null,
                    Errors = new List<string> { "User is not logged in." }
                };
            }
            var returnResponse = await matchService.AddMatches(userId, loggedInUserId);
            return new ReturnResponse<List<MatchResponse>>
            {
                StatusCode = returnResponse.StatusCode,
                Data = returnResponse.Data ?? null,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>()
            };
        }

        // GET: api/Match/all/{userId}   
        [HttpGet("all/{userId}")]
        [Authorize]
        public async Task<ActionResult<ReturnResponse<List<MatchResponse>>>> GetAllMatches(string userId)
        {
            var loggedInUserId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
            if (loggedInUserId == null)
            {
                return new ReturnResponse<List<MatchResponse>>
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = "Log in to access this resource.",
                    Data = null,
                    Errors = new List<string> { "User is not logged in." }
                };
            }
            var returnResponse = await matchService.GetMatchesByUserIdAsync(userId, loggedInUserId);
            return new ReturnResponse<List<MatchResponse>>
            {
                StatusCode = returnResponse.StatusCode,
                Data = returnResponse.Data ?? null,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>()
            };
        }

        [HttpDelete("{matchId}")]
        [Authorize]
        public async Task<ActionResult<ReturnResponse>> DeleteMatch(string matchId)
        {
            var loggedInUserId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
            if (loggedInUserId == null)
            {
                return new ReturnResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = "Log in to access this resource.",
                    Errors = new List<string> { "User is not logged in." }
                };
            }
            var returnResponse = await matchService.DeleteMatchAsync(matchId, loggedInUserId);
            return new ReturnResponse
            {
                StatusCode = returnResponse.StatusCode,
                Message = returnResponse.Message,
                Errors = returnResponse.Errors ?? new List<string>()
            };
        }
    }
}
