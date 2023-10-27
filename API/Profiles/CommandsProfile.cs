using API.Dtos;
using API.Model;
using AutoMapper;

namespace API.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            //Source --> target
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<CommandUpdateDto, Command>();
        }
    }
}