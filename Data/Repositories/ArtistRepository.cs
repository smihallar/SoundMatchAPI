using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Interfaces;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class ArtistRepository : GenericRepository<Artist>, IArtistRepository
    {
        private readonly ApplicationDbContext ctx;
        public ArtistRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async Task<Artist?> GetArtistWithDetailsAsync(string artistId)
        {
            return await ctx.Artists
                .Include(a => a.Genres)
                .FirstOrDefaultAsync(a => a.ArtistId == artistId);
        }

        public async Task<IEnumerable<Artist>> GetByIdsAsync(IEnumerable<string> artistIds)
        {
            return await ctx.Artists
                .Where(a => artistIds.Contains(a.ArtistId))
                .ToListAsync();
        }
    }
}
