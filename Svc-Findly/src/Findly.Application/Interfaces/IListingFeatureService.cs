using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

public interface IListingFeatureService
{
    Task<IEnumerable<ListingFeatureResponse>> GetByListingAsync(int listingId, CancellationToken cancellationToken = default);
    Task<ListingFeatureResponse> AddAsync(int listingId, AddListingFeatureRequest request, CancellationToken cancellationToken = default);
    Task<ListingFeatureResponse> UpdateAsync(int id, UpdateListingFeatureRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
