using Microsoft.AspNetCore.SignalR;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;

namespace SoundMatchAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;

        public ChatHub(IChatService chatService)
        {
            this.chatService = chatService;
        }
        public async Task SendMessage(string chatId, string senderId, string messageContent)
        {
            await chatService.AddMessageAsync(chatId, senderId, messageContent);
            // Broadcast to all clients in this chat group
            await Clients.Group(chatId).SendAsync("ReceiveMessage", senderId, messageContent, DateTime.UtcNow);
        }
    }
}
