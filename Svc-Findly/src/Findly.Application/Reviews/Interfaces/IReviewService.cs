using Findly.Contracts.Common;
using Findly.Contracts.Review.Requests;
using Findly.Contracts.Review.Responses;
using Findly.Domain.Enums;

namespace Findly.Application.Reviews.Interfaces;

public interface IReviewService
{
    Task<ReviewResponse>                GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResponse<ReviewResponse>> GetByListingAsync(int listingId, ReviewStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResponse<ReviewResponse>> GetAllAsync(ReviewStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ReviewResponse>                CreateAsync(int listingId, CreateReviewRequest request, CancellationToken cancellationToken = default);
    Task<ReviewResponse>                UpdateAsync(int id, UpdateReviewRequest request, CancellationToken cancellationToken = default);
    Task                                DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<ReviewResponse>                ApproveAsync(int id, CancellationToken cancellationToken = default);
    Task<ReviewResponse>                RejectAsync(int id, string reason, CancellationToken cancellationToken = default);
}
