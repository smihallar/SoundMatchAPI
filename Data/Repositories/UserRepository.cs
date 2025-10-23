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

        public async Task UpdateUserSpotifyStatusAsync(string userId, bool isConnectedToSpotify)
        {
            var user = await ctx.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsConnectedToSpotify = isConnectedToSpotify;
                ctx.Users.Update(user);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
