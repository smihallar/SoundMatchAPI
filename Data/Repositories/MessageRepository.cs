using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Repositories
{
    public class MessageRepository : GenericRepository<Message>
    {
        public MessageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
