using AutoMapper;
using Findly.Domain.Entities;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Mappings;

public class DbReviewMappingProfile : Profile
{
    public DbReviewMappingProfile()
    {
        CreateMap<DbReview, Review>();
        CreateMap<Review, DbReview>();
    }
}
