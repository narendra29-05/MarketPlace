using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface IReviewVoteRepository
{
    Task<ReviewVote?> GetByReviewAndUserAsync(int reviewId, int userId, CancellationToken cancellationToken);
    Task<IEnumerable<ReviewVote>> GetByReviewAsync(int reviewId, CancellationToken cancellationToken);
    Task<ReviewVote> CreateAsync(ReviewVote vote, CancellationToken cancellationToken);
    Task<ReviewVote> UpdateAsync(ReviewVote vote, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
