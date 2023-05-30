using AutoMapper;
using BotterApi.Models;

namespace BotterApi.Mapper.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<Post, PostDTO>().ReverseMap();
    }
}
