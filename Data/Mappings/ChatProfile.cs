using AutoMapper;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Models;

namespace SoundMatchAPI.Data.Mappings
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Chat, ChatResponse>()
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants));
        }
    }
}
