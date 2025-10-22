using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext ctx;
        public UserRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        //public async Task<User?> GetUserWithDetailsAsync(string id)
        //{
        //    return await ctx.Users
        //        .Include(u => u.FavoriteSongIds)
        //        .Include(u => u.FavoriteArtistIds)
        //        .Include(u => u.FavoriteGenreIds)
        //        .FirstOrDefaultAsync(u => u.Id == id);
        //}

        //public async Task<IEnumerable<User?>> GetAllUsersWithDetailsAsync()
        //{
        //    return await ctx.Users
        //        .Include(u => u.FavoriteSongIds)
        //        .Include(u => u.FavoriteArtistIds)
        //        .Include(u => u.FavoriteGenreIds)
        //        .ToListAsync();
        //}
    }
}
