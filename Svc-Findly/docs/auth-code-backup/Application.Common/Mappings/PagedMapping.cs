using AutoMapper;
using Findly.Contracts.Common;
using Findly.Domain.Repositories;

namespace Findly.Application.Common.Mappings;

public static class PagedMapping
{
    public static PagedResponse<TResponse> ToPagedResponse<TDomain, TResponse>(this PagedResult<TDomain> result, IMapper mapper)
    {
        return new PagedResponse<TResponse>
        {
            Items      = mapper.Map<List<TResponse>>(result.Items),
            TotalCount = result.TotalCount,
            Page       = result.Page,
            PageSize   = result.PageSize
        };
    }
}
