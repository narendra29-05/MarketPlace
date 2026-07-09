using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

public interface ITagService
{
    Task<TagResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TagResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<TagResponse> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken = default);
    Task<TagResponse> UpdateAsync(int id, UpdateTagRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
