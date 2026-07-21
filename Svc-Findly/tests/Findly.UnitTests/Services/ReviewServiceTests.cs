using AutoMapper;
using Findly.Application;
using Findly.Application.Common.Interfaces;
using Findly.Application.Reviews.Services;
using Findly.Contracts.Review.Requests;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Findly.UnitTests.Services;

public class ReviewServiceTests
{
    private readonly IReviewRepository  _reviewRepository  = Substitute.For<IReviewRepository>();
    private readonly IListingRepository _listingRepository = Substitute.For<IListingRepository>();
    private readonly IUnitOfWork        _unitOfWork        = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUser       _currentUser       = Substitute.For<ICurrentUser>();
    private readonly ReviewService      _service;

    public ReviewServiceTests()
    {
        var mapper = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(ApplicationModule).Assembly),
            NullLoggerFactory.Instance).CreateMapper();

        // Run the transactional action inline.
        _unitOfWork
            .ExecuteInTransactionAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => ((Func<Task>)callInfo[0])());

        _currentUser.Email.Returns("bob@corp.io");
        _currentUser.Name.Returns("Bob Buyer");
        _currentUser.IsAdmin.Returns(false);
        _currentUser.AuditName.Returns("bob@corp.io");

        _service = new ReviewService(_reviewRepository, _listingRepository, _unitOfWork, _currentUser, mapper);
    }

    private static Listing PublishedListing()
    {
        var listing = Listing.Create(
            "AcmeCRM", "acme-crm", "CRM", "https://acme.io", 1, PricingType.Paid, "t",
            null, null, null, null, null, null, null, false, null);
        listing.Approve("admin");
        return listing;
    }

    private static CreateReviewRequest Request() => new()
    {
        OverallRating         = 5,
        FeaturesRating        = 4,
        ValueForMoneyRating   = 5,
        CustomerSupportRating = 4,
        Title                 = "Great",
        Body                  = "Solid tool."
    };

    [Fact]
    public async Task Create_on_unpublished_listing_throws()
    {
        var pending = Listing.Create(
            "X", "x", "d", "https://x.io", 1, PricingType.Free, "t",
            null, null, null, null, null, null, null, false, null);
        _listingRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(pending);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(1, Request()));
    }

    [Fact]
    public async Task Create_twice_by_same_account_throws()
    {
        _listingRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(PublishedListing());
        _reviewRepository.GetByListingAndEmailAsync(1, "bob@corp.io", Arg.Any<CancellationToken>())
            .Returns(Review.Create(1, "Bob Buyer", "bob@corp.io", 5, 5, 5, 5, "T", "B", null, null, "x"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(1, Request()));
    }

    [Fact]
    public async Task Create_uses_account_identity_as_reviewer()
    {
        _listingRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(PublishedListing());
        _reviewRepository.GetByListingAndEmailAsync(1, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Review?)null);
        _reviewRepository.CreateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => (Review)callInfo[0]);

        var response = await _service.CreateAsync(1, Request());

        Assert.Equal("Bob Buyer", response.ReviewerName);
        Assert.Equal(ReviewStatus.Pending, response.Status);
    }

    [Fact]
    public async Task Update_by_non_author_throws()
    {
        var someoneElses = Review.Create(1, "Carol", "carol@ops.io", 5, 4, 5, 4, "T", "B", null, null, "x");
        _reviewRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(someoneElses);

        var request = new UpdateReviewRequest
        {
            OverallRating = 4, FeaturesRating = 4, ValueForMoneyRating = 4, CustomerSupportRating = 4,
            Title = "x", Body = "y"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateAsync(10, request));
    }

    [Fact]
    public async Task Approve_recomputes_listing_aggregates_in_transaction()
    {
        _currentUser.IsAdmin.Returns(true);
        var review  = Review.Create(1, "Bob Buyer", "bob@corp.io", 5, 4, 5, 4, "T", "B", null, null, "x");
        var listing = PublishedListing();

        _reviewRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(review);
        _listingRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(listing);
        _reviewRepository.GetApprovedAggregateAsync(1, Arg.Any<CancellationToken>())
            .Returns(new ListingRatingAggregate(4.50m, 4.00m, 4.50m, 4.00m, 3));

        var response = await _service.ApproveAsync(10);

        Assert.Equal(ReviewStatus.Approved, response.Status);
        Assert.Equal(4.50m, listing.AverageRating);
        Assert.Equal(3, listing.ReviewCount);
        await _listingRepository.Received(1).UpdateAsync(listing, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).ExecuteInTransactionAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_of_approved_review_recomputes_aggregates()
    {
        var review = Review.Create(1, "Bob Buyer", "bob@corp.io", 5, 4, 5, 4, "T", "B", null, null, "x");
        review.Approve("admin");
        var listing = PublishedListing();

        _reviewRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(review);
        _listingRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(listing);
        _reviewRepository.GetApprovedAggregateAsync(1, Arg.Any<CancellationToken>())
            .Returns(new ListingRatingAggregate(0m, 0m, 0m, 0m, 0));

        await _service.DeleteAsync(10);

        Assert.Equal(0, listing.ReviewCount);
        await _unitOfWork.Received(1).ExecuteInTransactionAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
    }
}
