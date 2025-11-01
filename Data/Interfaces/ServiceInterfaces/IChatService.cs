using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Interfaces.ServiceInterfaces
{
    public interface IChatService
    {
        Task<ReturnResponse<ChatResponse>> GetOrCreateChatAsync(string matchId, List<User> participants);
        Task<ReturnResponse> AddMessageAsync(string chatId, string senderId, string content);
        Task<ReturnResponse<List<ChatResponse>>> GetChatsByUserIdAsync(string userId);
    }
}
