using AutoMapper;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Mappings
{
    public class MatchProfile : Profile
    {
        public MatchProfile()
        {
            // Mapping from Match (source) to MatchResponse (destination). If null -> empty list
            CreateMap<Match, MatchResponse>()
                .ForMember(
                    d => d.MutualSongIds,
                    o => o.MapFrom(s => s.MutualSongs != null ? s.MutualSongs.Select(song => song.SongId).ToList() : new List<string>()))
                .ForMember(
                    d => d.MutualArtistIds,
                    o => o.MapFrom(s => s.MutualArtists != null ? s.MutualArtists.Select(a => a.ArtistId).ToList() : new List<string>()))
                .ForMember(
                    d => d.MutualGenreIds,
                    o => o.MapFrom(s => s.MutualGenres != null ? s.MutualGenres.Select(g => g.GenreId).ToList() : new List<string>()))
                .ReverseMap();
        }
    }
}
