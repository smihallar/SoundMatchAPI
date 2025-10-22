using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;

namespace SoundMatchAPI.Services
{
    public class SpotifyService
    {
        private readonly IUserService userService;

        public SpotifyService(IUserService userService)
        {
            this.userService = userService;
        }
    }
}
