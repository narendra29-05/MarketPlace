using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

public interface IListingMediaService
{
    Task<IEnumerable<ListingMediaResponse>> GetByListingAsync(int listingId, CancellationToken cancellationToken = default);
    Task<ListingMediaResponse> AddAsync(int listingId, AddListingMediaRequest request, CancellationToken cancellationToken = default);
    Task<ListingMediaResponse> UpdateAsync(int id, UpdateListingMediaRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
