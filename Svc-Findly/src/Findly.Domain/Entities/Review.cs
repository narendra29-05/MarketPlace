using Findly.Domain.Enums;

namespace Findly.Domain.Entities;

public class Review : Entity
{
    protected Review() { }

    public int          ListingId             { get; private set; }
    public string       ReviewerName          { get; private set; }
    public string       ReviewerEmail         { get; private set; }
    public int          OverallRating         { get; private set; }
    public int          FeaturesRating        { get; private set; }
    public int          ValueForMoneyRating   { get; private set; }
    public int          CustomerSupportRating { get; private set; }
    public string       Title                 { get; private set; }
    public string       Body                  { get; private set; }
    public string?      Pros                  { get; private set; }
    public string?      Cons                  { get; private set; }
    public ReviewStatus Status                { get; private set; }
    public string?      RejectionReason       { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static Review Create(
        int     listingId,
        string  reviewerName,
        string  reviewerEmail,
        int     overallRating,
        int     featuresRating,
        int     valueForMoneyRating,
        int     customerSupportRating,
        string  title,
        string  body,
        string? pros,
        string? cons,
        string  createdBy)
    {
        ValidateRating(overallRating,         "Overall rating");
        ValidateRating(featuresRating,        "Features rating");
        ValidateRating(valueForMoneyRating,   "Value for money rating");
        ValidateRating(customerSupportRating, "Customer support rating");

        return new Review
        {
            ListingId             = listingId,
            ReviewerName          = reviewerName,
            ReviewerEmail         = reviewerEmail.Trim().ToLowerInvariant(),
            OverallRating         = overallRating,
            FeaturesRating        = featuresRating,
            ValueForMoneyRating   = valueForMoneyRating,
            CustomerSupportRating = customerSupportRating,
            Title                 = title,
            Body                  = body,
            Pros                  = pros,
            Cons                  = cons,
            Status                = ReviewStatus.Pending,
            CreatedBy             = createdBy,
            CreatedAt             = DateTime.UtcNow,
            UpdatedAt             = DateTime.UtcNow
        };
    }

    // =========================================================================
    // Domain Methods
    // =========================================================================

    public void UpdateContent(
        int     overallRating,
        int     featuresRating,
        int     valueForMoneyRating,
        int     customerSupportRating,
        string  title,
        string  body,
        string? pros,
        string? cons,
        string  updatedBy)
    {
        if (Status == ReviewStatus.Approved)
            throw new InvalidOperationException("Approved reviews cannot be edited.");

        ValidateRating(overallRating,         "Overall rating");
        ValidateRating(featuresRating,        "Features rating");
        ValidateRating(valueForMoneyRating,   "Value for money rating");
        ValidateRating(customerSupportRating, "Customer support rating");

        OverallRating         = overallRating;
        FeaturesRating        = featuresRating;
        ValueForMoneyRating   = valueForMoneyRating;
        CustomerSupportRating = customerSupportRating;
        Title                 = title;
        Body                  = body;
        Pros                  = pros;
        Cons                  = cons;
        Status                = ReviewStatus.Pending;
        RejectionReason       = null;
        UpdatedAt             = DateTime.UtcNow;
        UpdatedBy             = updatedBy;
    }

    public void Approve(string updatedBy)
    {
        if (Status != ReviewStatus.Pending)
            throw new InvalidOperationException($"Cannot approve a review from status '{Status}'.");

        Status          = ReviewStatus.Approved;
        RejectionReason = null;
        UpdatedAt       = DateTime.UtcNow;
        UpdatedBy       = updatedBy;
    }

    public void Reject(string reason, string updatedBy)
    {
        if (Status != ReviewStatus.Pending)
            throw new InvalidOperationException($"Cannot reject a review from status '{Status}'.");

        Status          = ReviewStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt       = DateTime.UtcNow;
        UpdatedBy       = updatedBy;
    }

    private static void ValidateRating(int rating, string name)
    {
        if (rating is < 1 or > 5)
            throw new ArgumentException($"{name} must be between 1 and 5.");
    }
}
