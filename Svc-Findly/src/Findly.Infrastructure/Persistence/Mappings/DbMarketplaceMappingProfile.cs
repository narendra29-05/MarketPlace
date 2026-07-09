using AutoMapper;
using Findly.Domain.Entities;
using Findly.Domain.ValueObjects;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Mappings;

/// <summary>
/// DB &lt;-&gt; Domain mappings for the marketplace entities.
/// Straight column-to-property maps, except User which carries the Email value object.
/// </summary>
public class DbMarketplaceMappingProfile : Profile
{
    public DbMarketplaceMappingProfile()
    {
        // ---- User (Email value object) ----
        CreateMap<DbUser, User>()
            .ForMember(d => d.EmailAddress, o => o.MapFrom(s => Email.Create(s.Email)));
        CreateMap<User, DbUser>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.EmailAddress.Value));

        // ---- Straightforward two-way maps ----
        CreateMap<DbCategory, Category>().ReverseMap();
        CreateMap<DbTag, Tag>().ReverseMap();
        CreateMap<DbListingCategory, ListingCategory>().ReverseMap();
        CreateMap<DbListingTag, ListingTag>().ReverseMap();
        CreateMap<DbListingFeature, ListingFeature>().ReverseMap();
        CreateMap<DbListingMedia, ListingMedia>().ReverseMap();
        CreateMap<DbReview, Review>().ReverseMap();
        CreateMap<DbReviewVote, ReviewVote>().ReverseMap();
        CreateMap<DbLead, Lead>().ReverseMap();
        CreateMap<DbBookmark, Bookmark>().ReverseMap();
    }
}
