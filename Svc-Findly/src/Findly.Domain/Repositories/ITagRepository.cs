using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface ITagRepository
{
    Task<Tag> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Tag>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Tag> CreateAsync(Tag tag, CancellationToken cancellationToken);
    Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
