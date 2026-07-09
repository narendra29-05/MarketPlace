using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface IListingTagRepository
{
    Task<IEnumerable<ListingTag>> GetByListingAsync(int listingId, CancellationToken cancellationToken);
    Task<ListingTag> CreateAsync(ListingTag link, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
