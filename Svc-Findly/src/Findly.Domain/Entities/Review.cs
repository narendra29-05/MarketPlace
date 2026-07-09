using Findly.Domain.Enums;

namespace Findly.Domain.Entities;

public class Review : Entity
{
    protected Review() { }

    // Relations
    public int ListingId { get; private set; }
    public int UserId { get; private set; }

    // Content
    public string Title { get; private set; } = null!;
    public string? Pros { get; private set; }
    public string? Cons { get; private set; }
    public string? Comment { get; private set; }

    // Ratings (1-5) — these feed Listing's aggregate rating fields.
    public decimal OverallRating { get; private set; }
    public decimal FeaturesRating { get; private set; }
    public decimal CustomerSupportRating { get; private set; }

    // Moderation
    public ReviewStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static Review Create(
        int listingId,
        int userId,
        string title,
        decimal overallRating,
        decimal featuresRating,
        decimal customerSupportRating,
        string createdBy,
        string? pros = null,
        string? cons = null,
        string? comment = null)
    {
        GuardRating(overallRating);
        GuardRating(featuresRating);
        GuardRating(customerSupportRating);

        return new Review
        {
            ListingId = listingId,
            UserId = userId,
            Title = title,
            OverallRating = overallRating,
            FeaturesRating = featuresRating,
            CustomerSupportRating = customerSupportRating,
            Pros = pros,
            Cons = cons,
            Comment = comment,
            Status = ReviewStatus.Pending,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // =========================================================================
    // Domain Methods
    // =========================================================================

    public void UpdateContent(
        string title,
        decimal overallRating,
        decimal featuresRating,
        decimal customerSupportRating,
        string updatedBy,
        string? pros,
        string? cons,
        string? comment)
    {
        if (Status == ReviewStatus.Published)
            throw new InvalidOperationException("Published reviews cannot be edited.");

        GuardRating(overallRating);
        GuardRating(featuresRating);
        GuardRating(customerSupportRating);

        Title = title;
        OverallRating = overallRating;
        FeaturesRating = featuresRating;
        CustomerSupportRating = customerSupportRating;
        Pros = pros;
        Cons = cons;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Approve(string updatedBy)
    {
        if (Status != ReviewStatus.Pending)
            throw new InvalidOperationException($"Cannot approve from status '{Status}'.");

        Status = ReviewStatus.Published;
        RejectionReason = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Reject(string reason, string updatedBy)
    {
        if (Status != ReviewStatus.Pending)
            throw new InvalidOperationException($"Cannot reject from status '{Status}'.");

        Status = ReviewStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    private static void GuardRating(decimal rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
    }
}
