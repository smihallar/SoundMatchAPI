using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class GenreRepository : GenericRepository<Genre>, IGenreRepository
    {
        private readonly ApplicationDbContext ctx;
        public GenreRepository(ApplicationDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }

        public async Task<IEnumerable<Genre>> GetByIdsAsync(IEnumerable<string> genreIds)
        {
            return await ctx.Genres
                .Where(g => genreIds.Contains(g.GenreId))
                .ToListAsync();
        }

        public async Task<Genre?> GetByNameAsync(string name) // Used to compare to JSON from Spotify
        {
            return await ctx.Genres
                .FirstOrDefaultAsync(g => g.Name.ToLower() == name.ToLower());
        }
    }
}
