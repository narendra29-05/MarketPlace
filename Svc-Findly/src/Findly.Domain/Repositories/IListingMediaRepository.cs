using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface IListingMediaRepository
{
    Task<ListingMedia> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<ListingMedia>> GetByListingAsync(int listingId, CancellationToken cancellationToken);
    Task<ListingMedia> CreateAsync(ListingMedia media, CancellationToken cancellationToken);
    Task<ListingMedia> UpdateAsync(ListingMedia media, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
