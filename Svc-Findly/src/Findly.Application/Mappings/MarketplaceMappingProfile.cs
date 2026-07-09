using AutoMapper;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;

namespace Findly.Application.Mappings;

/// <summary>
/// Domain entity -> API response mappings for the marketplace resources.
/// </summary>
public class MarketplaceMappingProfile : Profile
{
    public MarketplaceMappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(d => d.EmailAddress, o => o.MapFrom(s => s.EmailAddress.Value));

        CreateMap<Category, CategoryResponse>();
        CreateMap<Tag, TagResponse>();
        CreateMap<Review, Findly.Contracts.Responses.ReviewResponse>();
        CreateMap<ReviewVote, ReviewVoteResponse>();
        CreateMap<Lead, LeadResponse>();
        CreateMap<Bookmark, BookmarkResponse>();
        CreateMap<ListingFeature, ListingFeatureResponse>();
        CreateMap<ListingMedia, ListingMediaResponse>();
        CreateMap<ListingCategory, ListingCategoryResponse>();
        CreateMap<ListingTag, ListingTagResponse>();
    }
}
