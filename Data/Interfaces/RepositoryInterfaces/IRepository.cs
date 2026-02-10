namespace SoundMatchAPI.Data.Interfaces.RepositoryInterfaces
{
    public interface IRepository<T> // Used for generic repository pattern
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(string id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
        Task SaveChangesAsync();
    }
}
