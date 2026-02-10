using AutoMapper;
using SoundMatchAPI.Data.DTOs.Responses.SpotifyAPIResponses;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Mappings
{
    public class SpotifyProfile : Profile
    {
        public SpotifyProfile()
        {
            // Artist mapping
            CreateMap<SpotifyArtistResponse, Artist>()
                .ForMember(dest => dest.SpotifyId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ArtistImageUrl, opt => opt.MapFrom(src => src.Images != null && src.Images.Any() ? src.Images.First().Url : string.Empty))
                .ForMember(dest => dest.Popularity, opt => opt.MapFrom(src => src.Popularity))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => new Genre { Name = g }).ToList()));

            // Song mapping
            CreateMap<SpotifyTrackResponse, Song>()
                .ForMember(dest => dest.SpotifyId, opt => opt.MapFrom(src => src.SpotifyId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AlbumImageUrl, opt => opt.MapFrom(src => src.Album.Images != null && src.Album.Images.Any() ? src.Album.Images.First().Url : string.Empty))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => src.Artists));
        }
    }
}
