using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.DomainModels
{
    public class MusicProfile
    {
        public IEnumerable<Song> Songs { get; }
        public IEnumerable<Artist> Artists { get; }
        public IEnumerable<Genre> Genres { get; }

        public MusicProfile(IEnumerable<Song> songs, IEnumerable<Artist> artists, IEnumerable<Genre> genres)
        {
            Songs = songs;
            Artists = artists;
            Genres = genres;
        }
    }
}
