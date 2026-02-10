using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.DTOs.Responses
{
    public class ChatResponse
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public List<User> Participants { get; set; } = new List<User>();
        public List<Message> Messages { get; set; } = new List<Message>();

        public string MatchId { get; set; } = string.Empty;
    }
}
