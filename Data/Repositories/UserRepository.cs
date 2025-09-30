
using SoundMatchAPI.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class UserRepository: GenericRepository<User>
    {
        private readonly ApplicationDbContext ctx;
        public UserRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }
    }
}
