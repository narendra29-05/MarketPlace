using AutoMapper;
using Findly.Domain.Entities;
using Findly.Contracts.Listing.Responses;

namespace Findly.Application.Listings.Mappings;

public class ListingMappingProfile : Profile
{
    public ListingMappingProfile()
    {
        CreateMap<Listing, ListingResponse>();
    }
}
