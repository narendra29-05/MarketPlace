using Findly.Domain.Entities;
using Findly.Domain.Enums;

namespace Findly.Domain.Repositories;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Listing?> GetBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<IReadOnlyList<Listing>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);

    Task<PagedResult<Listing>> GetAllAsync(int page, int pageSize, ListingStatus? status, CancellationToken cancellationToken);

    Task<PagedResult<Listing>> GetByVendorIdAsync(int vendorId, ListingStatus? status, int page, int pageSize, CancellationToken cancellationToken);

    Task<PagedResult<Listing>> SearchAsync(ListingSearchCriteria criteria, CancellationToken cancellationToken);

    Task<Listing> CreateAsync(Listing listing, CancellationToken cancellationToken);

    Task<Listing> UpdateAsync(Listing listing, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

    Task ReplaceCategoriesAsync(int listingId, IReadOnlyCollection<int> categoryIds, CancellationToken cancellationToken);
}
