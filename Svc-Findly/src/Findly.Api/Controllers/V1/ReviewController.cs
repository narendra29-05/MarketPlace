namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for review management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewController"/> class.
    /// </summary>
    /// <param name="reviewService">The review service.</param>
    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Gets a review by its identifier.
    /// </summary>
    /// <param name="id">The review identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The requested review.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _reviewService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a paged list of reviews for a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paged list of reviews for the listing.</returns>
    [HttpGet("~/api/v{version:apiVersion}/listings/{listingId:int}/reviews")]
    public async Task<IActionResult> GetByListing(int listingId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _reviewService.GetByListingAsync(listingId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new review.
    /// </summary>
    /// <param name="request">The request containing the review details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created review.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var review = await _reviewService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
    }

    /// <summary>
    /// Updates an existing review.
    /// </summary>
    /// <param name="id">The review identifier.</param>
    /// <param name="request">The request containing the updated review details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated review.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _reviewService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Approves a review.
    /// </summary>
    /// <param name="id">The review identifier.</param>
    /// <param name="request">The request containing the actor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The approved review.</returns>
    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] ApproveReviewRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _reviewService.ApproveAsync(id, request.UpdatedBy, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Rejects a review.
    /// </summary>
    /// <param name="id">The review identifier.</param>
    /// <param name="request">The request containing the rejection reason and actor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rejected review.</returns>
    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectReviewRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _reviewService.RejectAsync(id, request.Reason, request.UpdatedBy, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Votes on a review.
    /// </summary>
    /// <param name="id">The review identifier.</param>
    /// <param name="request">The request containing the vote details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The review with the recorded vote.</returns>
    [HttpPost("{id:int}/vote")]
    public async Task<IActionResult> Vote(int id, [FromBody] VoteReviewRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _reviewService.VoteAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a review.
    /// </summary>
    /// <param name="id">The review identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _reviewService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
