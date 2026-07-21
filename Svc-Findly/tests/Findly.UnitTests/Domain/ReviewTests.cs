using Findly.Domain.Entities;
using Findly.Domain.Enums;

namespace Findly.UnitTests.Domain;

public class ReviewTests
{
    private static Review NewReview() =>
        Review.Create(1, "Bob Buyer", "Bob@Corp.IO", 5, 4, 5, 4, "Great", "Solid tool.", "Easy", "Pricey", "system");

    [Fact]
    public void Create_starts_pending_with_normalized_email()
    {
        var review = NewReview();

        Assert.Equal(ReviewStatus.Pending, review.Status);
        Assert.Equal(1, review.ListingId);
        Assert.Equal("Bob Buyer", review.ReviewerName);
        Assert.Equal("bob@corp.io", review.ReviewerEmail);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Create_with_out_of_range_rating_throws(int rating)
    {
        Assert.Throws<ArgumentException>(() =>
            Review.Create(1, "X", "x@y.io", rating, 4, 5, 4, "T", "B", null, null, "system"));
    }

    [Fact]
    public void Approve_pending_approves()
    {
        var review = NewReview();

        review.Approve("system");

        Assert.Equal(ReviewStatus.Approved, review.Status);
    }

    [Fact]
    public void Approve_twice_throws()
    {
        var review = NewReview();
        review.Approve("system");

        Assert.Throws<InvalidOperationException>(() => review.Approve("system"));
    }

    [Fact]
    public void Reject_stores_reason()
    {
        var review = NewReview();

        review.Reject("Spam", "system");

        Assert.Equal(ReviewStatus.Rejected, review.Status);
        Assert.Equal("Spam", review.RejectionReason);
    }

    [Fact]
    public void UpdateContent_on_rejected_resets_to_pending()
    {
        var review = NewReview();
        review.Reject("Spam", "system");

        review.UpdateContent(4, 4, 4, 4, "Edited", "Better text", null, null, "system");

        Assert.Equal(ReviewStatus.Pending, review.Status);
        Assert.Null(review.RejectionReason);
        Assert.Equal("Edited", review.Title);
    }

    [Fact]
    public void UpdateContent_on_approved_throws()
    {
        var review = NewReview();
        review.Approve("system");

        Assert.Throws<InvalidOperationException>(() =>
            review.UpdateContent(4, 4, 4, 4, "Edited", "Text", null, null, "system"));
    }
}
