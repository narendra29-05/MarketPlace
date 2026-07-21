using AutoMapper;
using Findly.Domain.Entities;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Mappings;

public class DbCategoryMappingProfile : Profile
{
    public DbCategoryMappingProfile()
    {
        CreateMap<DbCategory, Category>();
        CreateMap<Category, DbCategory>();
    }
}
