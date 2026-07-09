using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface IListingFeatureRepository
{
    Task<ListingFeature> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<ListingFeature>> GetByListingAsync(int listingId, CancellationToken cancellationToken);
    Task<ListingFeature> CreateAsync(ListingFeature feature, CancellationToken cancellationToken);
    Task<ListingFeature> UpdateAsync(ListingFeature feature, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
