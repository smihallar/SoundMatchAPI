using AutoMapper;
using SoundMatchAPI.Data.DTOs.Requests;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Mapping from User (source) to UserResponse (destination)
            CreateMap<User, UserResponse>()
                .ForMember(
                    d => d.FavoriteSongIds,
                    o => o.MapFrom(s => s.FavoriteSongs.Select(s => s.SongId).ToList()))
                .ForMember(
                    d => d.FavoriteArtistIds,
                    o => o.MapFrom(s => s.FavoriteArtists.Select(a => a.ArtistId).ToList()))
                .ForMember(
                    d => d.FavoriteGenreIds,
                    o => o.MapFrom(s => s.FavoriteGenres.Select(g => g.GenreId).ToList())
                );
            // Mapping from UserCreateRequest (source) to User (destination)
            CreateMap<UserRegisterRequest, User>();
            CreateMap<User, UserProfileResponse>()
                .ForMember(
                    d => d.FavoriteSongs,
                    o => o.MapFrom(s => s.FavoriteSongs))
                .ForMember(
                    d => d.FavoriteArtists,
                    o => o.MapFrom(s => s.FavoriteArtists))
                .ForMember(
                    d => d.FavoriteGenres,
                    o => o.MapFrom(s => s.FavoriteGenres)
                );
        }
    }
}
