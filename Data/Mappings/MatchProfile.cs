using AutoMapper;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Mappings
{
    public class MatchProfile : Profile
    {
        public MatchProfile()
        {
            // Mapping from Match (source) to MatchResponse (destination) & reversed. If null -> empty list
            CreateMap<Match, MatchResponse>()
                .ForMember(
                    d => d.MutualSongs,
                    o => o.MapFrom(s => s.MutualSongs ?? new List<Song>()))
                .ForMember(
                    d => d.MutualArtists,
                    o => o.MapFrom(s => s.MutualArtists ?? new List<Artist>()))
                .ForMember(
                    d => d.MutualGenres,
                    o => o.MapFrom(s => s.MutualGenres ?? new List<Genre>()))
                .ReverseMap();
        }
    }
}
