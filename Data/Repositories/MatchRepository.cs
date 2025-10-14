using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class MatchRepository : GenericRepository<Match>
    {
        private readonly ApplicationDbContext ctx;
        public MatchRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async Task<IEnumerable<Match?>> GetMatchesWithDetailsByUserId(string id)
        {
            return await ctx.Matches
                .Include(m => m.InitiatorUser)
                    .ThenInclude(u => u.FavoriteSongs)
                        .ThenInclude(s => s.Artists)
                            .ThenInclude(a => a.Genres)
                .Include(m => m.InitiatorUser)
                    .ThenInclude(u => u.FavoriteArtists)
                        .ThenInclude(a => a.Genres)
                            .Include(m => m.InitiatorUser)
                                .ThenInclude(u => u.FavoriteGenres)
                .Include(m => m.RecipientUser)
                    .ThenInclude(u => u.FavoriteSongs)
                        .ThenInclude(s => s.Artists)
                            .ThenInclude(a => a.Genres)
                .Include(m => m.RecipientUser)
                    .ThenInclude(u => u.FavoriteArtists)
                        .ThenInclude(a => a.Genres)
                            .Include(m => m.RecipientUser)
                                .ThenInclude(u => u.FavoriteGenres)
                .Include(m => m.MutualSongs)
                .Include(m => m.MutualArtists)
                .Include(m => m.MutualGenres)
                .Where(m => m.InitiatorUserId == id || m.RecipientUserId == id)
                .ToListAsync();
        }
    }
}
