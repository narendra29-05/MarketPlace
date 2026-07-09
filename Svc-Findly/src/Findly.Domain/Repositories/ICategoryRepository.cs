using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Category>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken);
    Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
