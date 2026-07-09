using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface IBookmarkRepository
{
    Task<Bookmark> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Bookmark>> GetByUserAsync(int userId, int page, int pageSize, CancellationToken cancellationToken);
    Task<Bookmark> CreateAsync(Bookmark bookmark, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
