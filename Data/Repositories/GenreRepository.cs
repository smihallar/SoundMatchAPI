using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class GenreRepository : GenericRepository<Genre>
    {
        private readonly ApplicationDbContext ctx;
        public GenreRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }
    }
}
