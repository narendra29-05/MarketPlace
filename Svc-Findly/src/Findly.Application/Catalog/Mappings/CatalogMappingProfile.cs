using AutoMapper;
using Findly.Contracts.Catalog.Responses;
using Findly.Domain.Entities;

namespace Findly.Application.Catalog.Mappings;

public class CatalogMappingProfile : Profile
{
    public CatalogMappingProfile()
    {
        CreateMap<Listing, ListingSummaryResponse>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());

        CreateMap<Listing, ListingDetailResponse>()
            .ForMember(dest => dest.VendorCompanyName, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore());

        CreateMap<Listing, CompareItemResponse>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
    }
}
