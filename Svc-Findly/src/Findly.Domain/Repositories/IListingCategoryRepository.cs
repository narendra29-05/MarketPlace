using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface IListingCategoryRepository
{
    Task<IEnumerable<ListingCategory>> GetByListingAsync(int listingId, CancellationToken cancellationToken);
    Task<ListingCategory> CreateAsync(ListingCategory link, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
