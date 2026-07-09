using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;
using DReview = Findly.Domain.Entities.Review;
using DReviewVote = Findly.Domain.Entities.ReviewVote;

namespace Findly.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ReviewService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ReviewResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await _uow.Reviews.GetByIdAsync(id, cancellationToken);
        if (review is null)
            throw new KeyNotFoundException($"Review with id {id} not found.");
        return _mapper.Map<ReviewResponse>(review);
    }

    public async Task<IEnumerable<ReviewResponse>> GetByListingAsync(int listingId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var reviews = await _uow.Reviews.GetByListingAsync(listingId, page, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<ReviewResponse>>(reviews);
    }

    public async Task<ReviewResponse> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var review = DReview.Create(
            request.ListingId,
            request.UserId,
            request.Title,
            request.OverallRating,
            request.FeaturesRating,
            request.CustomerSupportRating,
            request.CreatedBy,
            request.Pros,
            request.Cons,
            request.Comment);

        var created = await _uow.Reviews.CreateAsync(review, cancellationToken);
        return _mapper.Map<ReviewResponse>(created);
    }

    public async Task<ReviewResponse> UpdateAsync(int id, UpdateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var review = await _uow.Reviews.GetByIdAsync(id, cancellationToken);
        if (review is null)
            throw new KeyNotFoundException($"Review with id {id} not found.");

        review.UpdateContent(
            request.Title,
            request.OverallRating,
            request.FeaturesRating,
            request.CustomerSupportRating,
            request.UpdatedBy,
            request.Pros,
            request.Cons,
            request.Comment);

        await _uow.Reviews.UpdateAsync(review, cancellationToken);
        return _mapper.Map<ReviewResponse>(review);
    }

    public async Task<ReviewResponse> ApproveAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var review = await _uow.Reviews.GetByIdAsync(id, cancellationToken);
        if (review is null)
            throw new KeyNotFoundException($"Review with id {id} not found.");

        review.Approve(updatedBy);
        await _uow.Reviews.UpdateAsync(review, cancellationToken);

        await RecomputeListingRatingsAsync(review.ListingId, cancellationToken);
        return _mapper.Map<ReviewResponse>(review);
    }

    public async Task<ReviewResponse> RejectAsync(int id, string reason, string updatedBy, CancellationToken cancellationToken = default)
    {
        var review = await _uow.Reviews.GetByIdAsync(id, cancellationToken);
        if (review is null)
            throw new KeyNotFoundException($"Review with id {id} not found.");

        review.Reject(reason, updatedBy);
        await _uow.Reviews.UpdateAsync(review, cancellationToken);
        return _mapper.Map<ReviewResponse>(review);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.Reviews.DeleteAsync(id, cancellationToken);
    }

    public async Task<ReviewVoteResponse> VoteAsync(int reviewId, VoteReviewRequest request, CancellationToken cancellationToken = default)
    {
        var review = await _uow.Reviews.GetByIdAsync(reviewId, cancellationToken);
        if (review is null)
            throw new KeyNotFoundException($"Review with id {reviewId} not found.");

        var existing = await _uow.ReviewVotes.GetByReviewAndUserAsync(reviewId, request.UserId, cancellationToken);
        if (existing is not null)
        {
            existing.ChangeVote(request.Vote, request.CreatedBy);
            await _uow.ReviewVotes.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<ReviewVoteResponse>(existing);
        }

        var vote = DReviewVote.Create(reviewId, request.UserId, request.Vote, request.CreatedBy);
        var created = await _uow.ReviewVotes.CreateAsync(vote, cancellationToken);
        return _mapper.Map<ReviewVoteResponse>(created);
    }

    // Recompute the listing's aggregate ratings from its published reviews.
    private async Task RecomputeListingRatingsAsync(int listingId, CancellationToken cancellationToken)
    {
        var listing = await _uow.Listings.GetByIdAsync(listingId, cancellationToken);
        if (listing is null) return;

        var published = (await _uow.Reviews.GetByListingAsync(listingId, 1, int.MaxValue, cancellationToken))
            .Where(r => r.Status == ReviewStatus.Published)
            .ToList();

        if (published.Count == 0)
        {
            listing.UpdateRatings(0, 0, 0, 0);
        }
        else
        {
            listing.UpdateRatings(
                Math.Round(published.Average(r => r.OverallRating), 2),
                Math.Round(published.Average(r => r.FeaturesRating), 2),
                Math.Round(published.Average(r => r.CustomerSupportRating), 2),
                published.Count);
        }

        await _uow.Listings.UpdateAsync(listing, cancellationToken);
    }
}
