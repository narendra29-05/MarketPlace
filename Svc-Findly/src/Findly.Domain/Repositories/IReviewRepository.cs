using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface IReviewRepository
{
    Task<Review> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Review>> GetByListingAsync(int listingId, int page, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<Review>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Review> CreateAsync(Review review, CancellationToken cancellationToken);
    Task<Review> UpdateAsync(Review review, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
