using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        private readonly ApplicationDbContext ctx;
        public UserRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async Task<User?> GetUserWithDetailsAsync(string id)
        {
            return await ctx.Users
                .Include(u => u.FavoriteSongs)
                    .ThenInclude(s => s.Artists)
                        .ThenInclude(a => a.Genres)
                .Include(u => u.FavoriteArtists)
                    .ThenInclude(a => a.Genres)
                .Include(u => u.FavoriteGenres)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User?>> GetAllUsersWithDetailsAsync()
        {
            return await ctx.Users
                .Include(u => u.FavoriteSongs)
                    .ThenInclude(s => s.Artists)
                        .ThenInclude(a => a.Genres)
                .Include(u => u.FavoriteArtists)
                    .ThenInclude(a => a.Genres)
                .Include(u => u.FavoriteGenres)
                .ToListAsync();
        }
    }
}
