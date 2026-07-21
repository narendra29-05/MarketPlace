using AutoMapper;
using Findly.Application.Common.Exceptions;
using Findly.Application.Common.Interfaces;
using Findly.Application.Common.Mappings;
using Findly.Application.Reviews.Interfaces;
using Findly.Contracts.Common;
using Findly.Contracts.Review.Requests;
using Findly.Contracts.Review.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;

namespace Findly.Application.Reviews.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository  _repository;
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork        _unitOfWork;
    private readonly ICurrentUser       _currentUser;
    private readonly IMapper            _mapper;

    public ReviewService(
        IReviewRepository  repository,
        IListingRepository listingRepository,
        IUnitOfWork        unitOfWork,
        ICurrentUser       currentUser,
        IMapper            mapper)
    {
        _repository        = repository;
        _listingRepository = listingRepository;
        _unitOfWork        = unitOfWork;
        _currentUser       = currentUser;
        _mapper            = mapper;
    }

    public async Task<ReviewResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await GetReviewOrThrowAsync(id, cancellationToken);
        return _mapper.Map<ReviewResponse>(review);
    }

    public async Task<PagedResponse<ReviewResponse>> GetByListingAsync(int listingId, ReviewStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page     = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await _repository.GetByListingAsync(listingId, status, page, pageSize, cancellationToken);
        return result.ToPagedResponse<Review, ReviewResponse>(_mapper);
    }

    public async Task<PagedResponse<ReviewResponse>> GetAllAsync(ReviewStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page     = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await _repository.GetAllAsync(status, page, pageSize, cancellationToken);
        return result.ToPagedResponse<Review, ReviewResponse>(_mapper);
    }

    public async Task<ReviewResponse> CreateAsync(int listingId, CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        // Reviewer identity comes from the authenticated account, not the request body.
        var reviewerEmail = _currentUser.Email
            ?? throw new AuthenticationFailedException("Not authenticated.");
        var reviewerName = _currentUser.Name ?? reviewerEmail;

        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {listingId} not found.");

        if (listing.Status != ListingStatus.Published)
            throw new InvalidOperationException("Reviews can only be submitted for published listings.");

        var existing = await _repository.GetByListingAndEmailAsync(listingId, reviewerEmail, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("You have already reviewed this listing.");

        var review = Review.Create(
            listingId,
            reviewerName,
            reviewerEmail,
            request.OverallRating,
            request.FeaturesRating,
            request.ValueForMoneyRating,
            request.CustomerSupportRating,
            request.Title,
            request.Body,
            request.Pros,
            request.Cons,
            _currentUser.AuditName);

        var created = await _repository.CreateAsync(review, cancellationToken);
        return _mapper.Map<ReviewResponse>(created);
    }

    public async Task<ReviewResponse> UpdateAsync(int id, UpdateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var review = await GetReviewOrThrowAsync(id, cancellationToken);
        EnsureAuthorOrAdmin(review);

        review.UpdateContent(
            request.OverallRating,
            request.FeaturesRating,
            request.ValueForMoneyRating,
            request.CustomerSupportRating,
            request.Title,
            request.Body,
            request.Pros,
            request.Cons,
            _currentUser.AuditName);

        await _repository.UpdateAsync(review, cancellationToken);
        return _mapper.Map<ReviewResponse>(review);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await GetReviewOrThrowAsync(id, cancellationToken);
        EnsureAuthorOrAdmin(review);

        if (review.Status == ReviewStatus.Approved)
        {
            // Removing an approved review changes the listing's aggregates.
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _repository.DeleteAsync(id, cancellationToken);
                await RecomputeAggregatesAsync(review.ListingId, cancellationToken);
            }, cancellationToken);
        }
        else
        {
            await _repository.DeleteAsync(id, cancellationToken);
        }
    }

    public async Task<ReviewResponse> ApproveAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await GetReviewOrThrowAsync(id, cancellationToken);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            review.Approve(_currentUser.AuditName);
            await _repository.UpdateAsync(review, cancellationToken);
            await RecomputeAggregatesAsync(review.ListingId, cancellationToken);
        }, cancellationToken);

        return _mapper.Map<ReviewResponse>(review);
    }

    public async Task<ReviewResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        var review = await GetReviewOrThrowAsync(id, cancellationToken);

        review.Reject(reason, _currentUser.AuditName);
        await _repository.UpdateAsync(review, cancellationToken);
        return _mapper.Map<ReviewResponse>(review);
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    private async Task<Review> GetReviewOrThrowAsync(int id, CancellationToken cancellationToken)
    {
        var review = await _repository.GetByIdAsync(id, cancellationToken);
        if (review is null)
            throw new KeyNotFoundException($"Review with id {id} not found.");

        return review;
    }

    private void EnsureAuthorOrAdmin(Review review)
    {
        if (_currentUser.IsAdmin)
            return;

        var email = _currentUser.Email;
        if (email is null || !string.Equals(review.ReviewerEmail, email, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("You can only manage your own reviews.");
    }

    private async Task RecomputeAggregatesAsync(int listingId, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {listingId} not found.");

        var aggregate = await _repository.GetApprovedAggregateAsync(listingId, cancellationToken);

        listing.UpdateRatings(
            aggregate.AverageRating,
            aggregate.FeaturesRating,
            aggregate.ValueForMoneyRating,
            aggregate.CustomerSupportRating,
            aggregate.ReviewCount);

        await _listingRepository.UpdateAsync(listing, cancellationToken);
    }
}
