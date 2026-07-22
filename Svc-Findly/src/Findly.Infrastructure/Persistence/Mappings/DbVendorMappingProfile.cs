using AutoMapper;
using Findly.Domain.Entities;
using Findly.Domain.ValueObjects;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Mappings;

public class DbVendorMappingProfile : Profile
{
    public DbVendorMappingProfile()
    {
        // DbVendor → Vendor  (read from DB)
        CreateMap<DbVendor, Vendor>()
            .ForMember(d => d.EmailAddress, o => o.MapFrom(s => Email.Create(s.Email)))
            .ForMember(d => d.Phone, o => o.MapFrom(s => PhoneNumber.Create(s.Phone)))
            .ForMember(d => d.Address, o => o.MapFrom(s =>
                Address.Create(s.AddressLine1, s.AddressLine2, s.City, s.State, s.Country, s.PostalCode)));

        // Vendor → DbVendor  (write to DB)
        CreateMap<Vendor, DbVendor>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.EmailAddress.Value))
            .ForMember(d => d.Phone, o => o.MapFrom(s => s.Phone.Value))
            .ForMember(d => d.AddressLine1, o => o.MapFrom(s => s.Address!.AddressLine1))
            .ForMember(d => d.AddressLine2, o => o.MapFrom(s => s.Address!.AddressLine2))
            .ForMember(d => d.City, o => o.MapFrom(s => s.Address!.City))
            .ForMember(d => d.State, o => o.MapFrom(s => s.Address!.State))
            .ForMember(d => d.Country, o => o.MapFrom(s => s.Address!.Country))
            .ForMember(d => d.PostalCode, o => o.MapFrom(s => s.Address!.PostalCode));
    }
}
