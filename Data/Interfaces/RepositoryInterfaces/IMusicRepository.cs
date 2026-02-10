namespace SoundMatchAPI.Data.Interfaces.RepositoryInterfaces
{
    public interface IMusicRepository<T>
    {
        Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<string> ids);
    }
}
