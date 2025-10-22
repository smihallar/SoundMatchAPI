using SoundMatchAPI.Data.DTOs.Responses;

namespace SoundMatchAPI.Data.Interfaces
{
    public interface IMatchService
    {
        Task<ReturnResponse<MatchResponse>> GetMatchByIdWithDetailsAsync(string matchId);
        Task<ReturnResponse<IEnumerable<MatchResponse>>> AddMatches(string userId);
        Task<ReturnResponse<IEnumerable<MatchResponse>>> GetMatchesByUserIdAsync(string userId);
        Task<ReturnResponse> DeleteMatchAsync(string matchId);

    }
}
