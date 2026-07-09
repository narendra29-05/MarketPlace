using Findly.Domain.Enums;

namespace Findly.Domain.Entities;

// A User's "was this review helpful?" vote. One vote per user per review.
public class ReviewVote : Entity
{
    protected ReviewVote() { }

    public int ReviewId { get; private set; }
    public int UserId { get; private set; }
    public VoteType Vote { get; private set; }

    public static ReviewVote Create(int reviewId, int userId, VoteType vote, string createdBy)
    {
        return new ReviewVote
        {
            ReviewId = reviewId,
            UserId = userId,
            Vote = vote,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void ChangeVote(VoteType vote, string updatedBy)
    {
        Vote = vote;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
