namespace SoundMatchAPI.Data.Interfaces
{
    public interface IMusicRepository<T>
    {
        Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<string> ids);
    }
}
