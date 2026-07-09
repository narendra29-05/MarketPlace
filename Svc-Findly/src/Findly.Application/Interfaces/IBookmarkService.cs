using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

public interface IBookmarkService
{
    Task<IEnumerable<BookmarkResponse>> GetByUserAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<BookmarkResponse> CreateAsync(CreateBookmarkRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
