using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class MessageRepository : GenericRepository<Message>
    {
        private readonly ApplicationDbContext ctx;

        public MessageRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async Task<List<Message>> GetMessagesByChatIdAsync(string chatId)
        {
            return await ctx.Messages
                .Where(m => m.ChatId == chatId)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
