using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundMatchAPI.Services;
using System.Net;

namespace SoundMatchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly MatchService matchService;

        public MatchController(MatchService matchService)
        {
            this.matchService = matchService;
        }

        // GET: api/Match/{matchId}
        [HttpGet("{matchId}")]
        public async Task<IActionResult> GetMatch(string matchId)
        {
            var returnResponse = await matchService.GetMatchByIdWithDetailsAsync(matchId);

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
        // POST: api/Match/{userId}
        [HttpPost("all/{userId}")]
        public async Task<IActionResult> CreateMatches(string userId)
        {
            var returnResponse = await matchService.AddMatches(userId);
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
            var returnResponse = await matchService.GetMatchesByUserIdAsync(userId);
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
            var returnResponse = await matchService.DeleteMatchAsync(matchId);
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
