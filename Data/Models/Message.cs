using System;

namespace SoundMatchAPI.Data.Models
{
    public class Message
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public string MessageContent { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Navigational properties
        public string SenderId { get; set; }
        public User Sender { get; set; }

        public string ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
