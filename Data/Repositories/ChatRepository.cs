using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class ChatRepository : GenericRepository<Chat>
    {
        public ChatRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
