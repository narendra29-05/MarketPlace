using AutoMapper;
using Findly.Domain.Entities;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Mappings;

public class DbListingMappingProfile : Profile
{
    public DbListingMappingProfile()
    {
        // DbListing → Listing  (read from DB — all columns match, no value objects)
        CreateMap<DbListing, Listing>();

        // Listing → DbListing  (write to DB)
        CreateMap<Listing, DbListing>();
    }
}
