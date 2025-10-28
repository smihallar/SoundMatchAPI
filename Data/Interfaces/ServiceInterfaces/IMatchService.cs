using SoundMatchAPI.Data.DTOs.Responses;

namespace SoundMatchAPI.Data.Interfaces.ServiceInterfaces
{
    public interface IMatchService
    {
        Task<ReturnResponse<MatchResponse>> GetMatchByIdWithDetailsAsync(string matchId, string loggedInUserId);
        Task<ReturnResponse<List<MatchResponse>>> AddMatches(string userId, string loggedInUserId);
        Task<ReturnResponse<List<MatchResponse>>> GetMatchesByUserIdAsync(string userId, string loggedInUserId);
        Task<ReturnResponse> DeleteMatchAsync(string matchId, string loggedInUserId);
    }
}
