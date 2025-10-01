using Microsoft.EntityFrameworkCore;
using SoundMatchAPI.Data.Interfaces;

namespace SoundMatchAPI.Data.Repositories
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext ctx;
        private DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext ctx)
        {
            this.ctx = ctx;
            this.dbSet = ctx.Set<T>();
        }
        public async virtual Task AddAsync(T entity)
        {
            
            dbSet.Add(entity);
            await SaveChangesAsync();
        }

        public async virtual Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return;
            dbSet.Remove(entity);
            await SaveChangesAsync();
        }

        public async virtual Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async virtual Task<T?> GetByIdAsync(string id)
        {
            return await dbSet.FindAsync(id);
        }

        public async virtual Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            await SaveChangesAsync();
        }

        public async virtual Task SaveChangesAsync()
        {
            await ctx.SaveChangesAsync();
        }
    }
}
