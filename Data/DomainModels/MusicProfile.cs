using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.DomainModels
{
    public class MusicProfile
    {
        public IEnumerable<Song> Songs { get; }
        public IEnumerable<Genre> Genres { get; }
        public IEnumerable<Artist> Artists { get; }

        public MusicProfile(IEnumerable<Song> songs, IEnumerable<Genre> genres, IEnumerable<Artist> artists)
        {
            Songs = songs;
            Genres = genres;
            Artists = artists;
        }
    }
}
