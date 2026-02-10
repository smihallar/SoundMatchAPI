using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace SoundMatchAPI.Data.Repositories
{
    public class ChatRepository : GenericRepository<Chat>, IChatRepository
    {
        private readonly ApplicationDbContext ctx;

        public ChatRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async Task<List<Chat>> GetChatsByUserIdAsync(string userId)
        {
            return await ctx.Chats
                .Where(c => c.Participants.Any(p => p.Id == userId))
                .Include(c => c.Participants)
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .ToListAsync();
        }

        public async Task<Chat?> GetChatByMatchIdAsync(string matchId)
        {
            return await ctx.Chats
                .Include(c => c.Participants)
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(c => c.MatchId == matchId);
        }
    }
}
