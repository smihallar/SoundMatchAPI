using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class ArtistRepository : GenericRepository<Artist>
    {
        private readonly ApplicationDbContext ctx;
        public ArtistRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async Task<Artist?> GetArtistWithDetailsAsync(string id)
        {
            return await ctx.Artists
                .Include(a => a.Genres)
                .FirstOrDefaultAsync(a => a.ArtistId == id);
        }
    }
}
