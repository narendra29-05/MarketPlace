using Findly.Domain.Enums;

namespace Findly.Contracts.Responses;

public class ReviewVoteResponse
{
    public int Id { get; set; }
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public VoteType Vote { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
