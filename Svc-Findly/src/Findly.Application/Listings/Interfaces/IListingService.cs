using Findly.Contracts.Common;
using Findly.Contracts.Listing.Requests;
using Findly.Contracts.Listing.Responses;
using Findly.Domain.Enums;

namespace Findly.Application.Listings.Interfaces;

public interface IListingService
{
    Task<ListingResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResponse<ListingResponse>> GetAllAsync(int page, int pageSize, ListingStatus? status = null, CancellationToken cancellationToken = default);
    Task<PagedResponse<ListingResponse>> GetMyListingsAsync(int page, int pageSize, ListingStatus? status = null, CancellationToken cancellationToken = default);
    Task<ListingResponse> CreateAsync(CreateListingRequest request, CancellationToken cancellationToken = default);
    Task<ListingResponse> UpdateAsync(int id, UpdateListingRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<ListingResponse> ApproveAsync(int id, CancellationToken cancellationToken = default);
    Task<ListingResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken = default);
    Task<ListingResponse> ArchiveAsync(int id, CancellationToken cancellationToken = default);
    Task<ListingResponse> RestoreAsync(int id, CancellationToken cancellationToken = default);
}
