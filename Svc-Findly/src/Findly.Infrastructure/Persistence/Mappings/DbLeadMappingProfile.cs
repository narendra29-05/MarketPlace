using AutoMapper;
using Findly.Domain.Entities;
using Findly.Domain.ValueObjects;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Mappings;

public class DbLeadMappingProfile : Profile
{
    public DbLeadMappingProfile()
    {
        CreateMap<DbLead, Lead>()
            .ForMember(d => d.BusinessEmail, o => o.MapFrom(s => Email.Create(s.BusinessEmail)))
            .ForMember(d => d.Phone, o => o.MapFrom(s => s.Phone != null ? PhoneNumber.Create(s.Phone) : null));

        CreateMap<Lead, DbLead>()
            .ForMember(d => d.BusinessEmail, o => o.MapFrom(s => s.BusinessEmail.Value))
            .ForMember(d => d.Phone, o => o.MapFrom(s => s.Phone != null ? s.Phone.Value : null));
    }
}
