using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {
        private readonly ApplicationDbContext ctx;
        public MatchRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public Task AddMatchesAsync(IEnumerable<Match> matches)
        {
            ctx.Matches.AddRange(matches);
            return ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<Match?>> GetMatchesWithDetailsByUserIdAsync(string userId)
        {
            return await ctx.Matches
                .Include(m => m.InitiatorUser)
                .Include(m => m.RecipientUser)
                .Include(m => m.MutualSongs)
                    .ThenInclude(s => s.Artists)
                        .ThenInclude(a => a.Genres)
                .Include(m => m.MutualArtists)
                    .ThenInclude(a => a.Genres)
                .Include(m => m.MutualGenres)
                .Where(m => m.InitiatorUserId == userId || m.RecipientUserId == userId)
                .ToListAsync();
        }
        public async Task<Match?> GetMatchWithDetailsByIdAsync(string matchId)
        {
            return await ctx.Matches
               .Include(m => m.InitiatorUser)
               .Include(m => m.RecipientUser)
               .Include(m => m.MutualSongs)
                   .ThenInclude(s => s.Artists)
                       .ThenInclude(a => a.Genres)
               .Include(m => m.MutualArtists)
                   .ThenInclude(a => a.Genres)
               .Include(m => m.MutualGenres)
           .FirstOrDefaultAsync(m => m.MatchId == matchId);
        }

        public async Task DeleteMatchesByUserIdAsync(string userId)
        {
            var matches = await ctx.Matches
                .Where(m => m.InitiatorUserId == userId || m.RecipientUserId == userId)
                .ToListAsync();
            ctx.Matches.RemoveRange(matches);
            await ctx.SaveChangesAsync();
        }

        // Check for existing match between two users
        public async Task<Match?> GetExistingMatchAsync(string initiatorId, string recipientId)
        {
            return await ctx.Matches
                .Include(m => m.MutualSongs)
                .Include(m => m.MutualArtists)
                .Include(m => m.MutualGenres)
            .FirstOrDefaultAsync(m =>
                m.InitiatorUserId == initiatorId &&
                m.RecipientUserId == recipientId
            );

        }
    }
}
