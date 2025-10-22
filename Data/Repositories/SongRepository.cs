using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class SongRepository : GenericRepository<Song>, ISongRepository
    {
        private readonly ApplicationDbContext ctx;
        public SongRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async Task<IEnumerable<Song>> GetByIdsAsync(IEnumerable<string> songIds)
        {
            return await ctx.Songs
                .Where(s => songIds.Contains(s.SongId))
                .Include(s => s.Artists)
                    .ThenInclude(a => a.Genres)
                .ToListAsync();
        }

        public async Task<IEnumerable<Song?>> GetSongsWithDetailsByIdsAsync(IEnumerable<string> songIds)
        {
            return await ctx.Songs
                .Where(s => songIds.Contains(s.SongId))
                .Include(s => s.Artists)
                    .ThenInclude(a => a.Genres)
                .ToListAsync();
        }

        public async Task<Song?> GetSongWithDetailsAsync(string songId)
        {
            return await ctx.Songs
                .Include(s => s.Artists)
                    .ThenInclude(a => a.Genres)
                .FirstOrDefaultAsync(s => s.SongId == songId);
        }
    }
}
