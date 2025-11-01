using AutoMapper;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository chatRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public ChatService(IChatRepository chatRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            this.chatRepository = chatRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }

        public async Task<ReturnResponse<ChatResponse>> GetOrCreateChatAsync(string matchId, List<User> participants)
        {
            try
            {
                var chat = await chatRepository.GetChatByMatchIdAsync(matchId);
                var chatResponse = mapper.Map<ChatResponse>(chat);
                if (chat != null)
                {
                    return new ReturnResponse<ChatResponse>
                    {
                        Data = chatResponse
                    };
                }

                var newChat = new Chat
                {
                    MatchId = matchId,
                    Participants = participants
                };

                await chatRepository.AddAsync(newChat);
                var newChatResponse = mapper.Map<ChatResponse>(newChat);
                return new ReturnResponse<ChatResponse>
                {
                    Data = newChatResponse
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<ChatResponse>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new List<string> { ex.Message },
                    Message = "Failed to get or create chat."
                };
            }
        }

        public async Task<ReturnResponse> AddMessageAsync(string chatId, string senderId, string content)
        {
            try
            {
                var chat = await chatRepository.GetByIdAsync(chatId);
                if (chat == null)
                {
                    return new ReturnResponse
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Message = "Chat not found."
                    };
                }
                var message = new Message
                {
                    ChatId = chatId,
                    SenderId = senderId,
                    MessageContent = content,
                    SentAt = DateTime.UtcNow
                };

                await messageRepository.AddAsync(message);

                chat.Messages.Add(message);
                await chatRepository.UpdateAsync(chat);

                return new ReturnResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Message added to chat successfully."
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new List<string> { ex.Message },
                    Message = "Failed to add message to chat."
                };
            }
        }
        public async Task<ReturnResponse<List<ChatResponse>>> GetChatsByUserIdAsync(string userId)
        {
            try
            {
                var chats = await chatRepository.GetChatsByUserIdAsync(userId);
                if (chats == null || !chats.Any())
                {
                    return new ReturnResponse<List<ChatResponse>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Message = "No chats found for the user.",
                        Errors = new List<string> { "User has no chats." }
                    };
                }

                var chatResponses = mapper.Map<List<ChatResponse>>(chats);
                return new ReturnResponse<List<ChatResponse>>
                {
                    Data = chatResponses
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<List<ChatResponse>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Errors = new List<string> { ex.Message },
                    Message = "Failed to retrieve chats for user."
                };
            }
        }
    }
}
