using AutoMapper;
using Findly.Contracts.Lead.Responses;
using Findly.Domain.Entities;

namespace Findly.Application.Leads.Mappings;

public class LeadMappingProfile : Profile
{
    public LeadMappingProfile()
    {
        CreateMap<Lead, LeadResponse>()
            .ForMember(dest => dest.BusinessEmail, opt => opt.MapFrom(src => src.BusinessEmail.Value))
            .ForMember(dest => dest.Phone,         opt => opt.MapFrom(src => src.Phone != null ? src.Phone.Value : null))
            .ForMember(dest => dest.ListingName,   opt => opt.Ignore());
    }
}
