using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface IListingRepository
{
    Task<Listing> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Listing>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Listing> CreateAsync(Listing listing, CancellationToken cancellationToken);
    Task<Listing> UpdateAsync(Listing listing, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
