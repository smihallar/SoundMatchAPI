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

        public async Task UpdateUserWithSpotifyAuthDetailsAsync(string userId, string? refreshToken, DateTime? tokenExpiresAt)
        {
            var user = await ctx.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsConnectedToSpotify = true;
                user.SpotifyRefreshToken = refreshToken;
                user.SpotifyTokenExpiresAt = tokenExpiresAt;
                ctx.Users.Update(user);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<User?> GetUserWithFavoriteMusic(string userId)
        {
            return await ctx.Users
                .Include(u=>u.FavoriteArtists)
                .Include(u=>u.FavoriteSongs)
                .Include(u=>u.FavoriteGenres)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
