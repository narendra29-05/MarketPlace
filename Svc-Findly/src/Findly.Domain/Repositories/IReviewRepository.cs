using Findly.Domain.Entities;
using Findly.Domain.Enums;

namespace Findly.Domain.Repositories;

public sealed record ListingRatingAggregate(
    decimal AverageRating,
    decimal FeaturesRating,
    decimal ValueForMoneyRating,
    decimal CustomerSupportRating,
    int     ReviewCount);

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Review?> GetByListingAndEmailAsync(int listingId, string reviewerEmail, CancellationToken cancellationToken);

    Task<PagedResult<Review>> GetByListingAsync(int listingId, ReviewStatus? status, int page, int pageSize, CancellationToken cancellationToken);

    Task<PagedResult<Review>> GetAllAsync(ReviewStatus? status, int page, int pageSize, CancellationToken cancellationToken);

    Task<Review> CreateAsync(Review review, CancellationToken cancellationToken);

    Task<Review> UpdateAsync(Review review, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<ListingRatingAggregate> GetApprovedAggregateAsync(int listingId, CancellationToken cancellationToken);
}
