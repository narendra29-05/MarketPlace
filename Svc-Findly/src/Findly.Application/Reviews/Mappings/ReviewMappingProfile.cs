using AutoMapper;
using Findly.Contracts.Review.Responses;
using Findly.Domain.Entities;

namespace Findly.Application.Reviews.Mappings;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewResponse>();
    }
}
