using AutoMapper;
using Findly.Domain.Entities;
using Findly.Domain.ValueObjects;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Mappings;

public class DbUserMappingProfile : Profile
{
    public DbUserMappingProfile()
    {
        CreateMap<DbUser, User>()
            .ForMember(d => d.EmailAddress, o => o.MapFrom(s => Email.Create(s.Email)));

        CreateMap<User, DbUser>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.EmailAddress.Value));
    }
}
