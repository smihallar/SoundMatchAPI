namespace SoundMatchAPI.Models
{
    public class Chat
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public List<User> Participants { get; set; } = new List<User>();
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
