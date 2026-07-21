using AutoMapper;
using Findly.Contracts.Category.Responses;
using Findly.Domain.Entities;

namespace Findly.Application.Categories.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryResponse>();
    }
}
