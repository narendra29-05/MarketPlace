using Asp.Versioning;
using Findly.Application.Reviews.Interfaces;
using Findly.Contracts.Common;
using Findly.Contracts.Review.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Findly.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost("api/v{version:apiVersion}/listings/{listingId:int}/reviews")]
    public async Task<IActionResult> Create(
        int listingId,
        [FromBody] CreateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var review = await _reviewService.CreateAsync(listingId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = review.Id, version = "1.0" }, review);
    }

    [HttpGet("api/v{version:apiVersion}/reviews/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var review = await _reviewService.GetByIdAsync(id, cancellationToken);
        return Ok(review);
    }

    [HttpPut("api/v{version:apiVersion}/reviews/{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var review = await _reviewService.UpdateAsync(id, request, cancellationToken);
        return Ok(review);
    }

    [HttpDelete("api/v{version:apiVersion}/reviews/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _reviewService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("api/v{version:apiVersion}/reviews/{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken = default)
    {
        var review = await _reviewService.ApproveAsync(id, cancellationToken);
        return Ok(review);
    }

    [HttpPost("api/v{version:apiVersion}/reviews/{id:int}/reject")]
    public async Task<IActionResult> Reject(
        int id,
        [FromBody] RejectRequest request,
        CancellationToken cancellationToken = default)
    {
        var review = await _reviewService.RejectAsync(id, request.Reason, cancellationToken);
        return Ok(review);
    }
}
