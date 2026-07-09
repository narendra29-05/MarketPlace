using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

public interface IListingService
{
    Task<ListingResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ListingResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ListingResponse> CreateAsync(CreateListingRequest request, CancellationToken cancellationToken = default);
    Task<ListingResponse> UpdateAsync(int id, UpdateListingRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<ListingResponse> ApproveAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<ListingResponse> RejectAsync(int id, string reason, string updatedBy, CancellationToken cancellationToken = default);
    Task<ListingResponse> ArchiveAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<ListingResponse> RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
