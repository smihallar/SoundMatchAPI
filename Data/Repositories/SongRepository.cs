using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class SongRepository : GenericRepository<Song>
    {
        private readonly ApplicationDbContext ctx;
        public SongRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async Task<Song?> GetSongWithDetailsAsync(string id)
        {
            return await ctx.Songs
                .Include(s => s.Artists)
                    .ThenInclude(a => a.Genres)
                .FirstOrDefaultAsync(s => s.SongId == id);
        }
    }
}
