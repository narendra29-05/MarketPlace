using AutoMapper;
using Findly.Contracts.Auth.Responses;
using Findly.Domain.Entities;

namespace Findly.Application.Auth.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress.Value));
    }
}
