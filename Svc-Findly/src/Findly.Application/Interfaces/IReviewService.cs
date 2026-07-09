using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

public interface IReviewService
{
    Task<ReviewResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReviewResponse>> GetByListingAsync(int listingId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ReviewResponse> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken = default);
    Task<ReviewResponse> UpdateAsync(int id, UpdateReviewRequest request, CancellationToken cancellationToken = default);
    Task<ReviewResponse> ApproveAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<ReviewResponse> RejectAsync(int id, string reason, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    // Sub-resources
    Task<ReviewVoteResponse> VoteAsync(int reviewId, VoteReviewRequest request, CancellationToken cancellationToken = default);
}
