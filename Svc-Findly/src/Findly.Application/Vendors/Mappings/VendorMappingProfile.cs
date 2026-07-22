using AutoMapper;
using Findly.Domain.Entities;
using Findly.Contracts.Vendor.Responses;

namespace Findly.Application.Vendors.Mappings;

public class VendorMappingProfile : Profile
{
    public VendorMappingProfile()
    {
        CreateMap<Vendor, VendorResponse>()
            .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.IndustryType))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress.Value))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone.Value))
            .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.Address != null ? src.Address.AddressLine1 : null))
            .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.Address != null ? src.Address.AddressLine2 : null))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address.City : null))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Address != null ? src.Address.State : null))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address != null ? src.Address.Country : null))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.Address != null ? src.Address.PostalCode : null));
    }
}
