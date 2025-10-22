using System;

namespace SoundMatchAPI.Data.Models
{
    public class Message
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public string MessageContent { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Navigational properties
        public string SenderId { get; set; } = string.Empty;
        public User Sender { get; set; } = new User();

        public string ChatId { get; set; } = string.Empty;
        public Chat Chat { get; set; } = new Chat();
    }
}
