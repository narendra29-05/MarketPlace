using Findly.Domain.Enums;

namespace Findly.Contracts.Requests;

public class CreateReviewRequest
{
    public int ListingId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public decimal OverallRating { get; set; }
    public decimal FeaturesRating { get; set; }
    public decimal CustomerSupportRating { get; set; }
    public string? Pros { get; set; }
    public string? Cons { get; set; }
    public string? Comment { get; set; }
    public string CreatedBy { get; set; } = null!;
}

public class UpdateReviewRequest
{
    public string Title { get; set; } = null!;
    public decimal OverallRating { get; set; }
    public decimal FeaturesRating { get; set; }
    public decimal CustomerSupportRating { get; set; }
    public string? Pros { get; set; }
    public string? Cons { get; set; }
    public string? Comment { get; set; }
    public string UpdatedBy { get; set; } = null!;
}

public class RejectReviewRequest
{
    public string Reason { get; set; } = null!;
    public string UpdatedBy { get; set; } = null!;
}

public class ApproveReviewRequest
{
    public string UpdatedBy { get; set; } = null!;
}

// "Was this review helpful?" vote
public class VoteReviewRequest
{
    public int UserId { get; set; }
    public VoteType Vote { get; set; }
    public string CreatedBy { get; set; } = null!;
}
