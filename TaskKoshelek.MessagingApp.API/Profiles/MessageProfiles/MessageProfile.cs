using AutoMapper;
using TaskKoshelek.MessagingApp.API.APIDtos.MessageDTOs;
using TaskKoshelek.MessagingApp.Core.Entities;

namespace TaskKoshelek.MessagingApp.API.Profiles.MessageProfiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageCreateDTO>().ReverseMap();
            CreateMap<Message, MessageDTO>().ReverseMap();
        }
    }
}
