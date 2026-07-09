using AutoMapper;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;

namespace Findly.Application.Mappings;

public class ListingMappingProfile : Profile
{
    public ListingMappingProfile()
    {
        CreateMap<Listing, ListingResponse>();
    }
}
