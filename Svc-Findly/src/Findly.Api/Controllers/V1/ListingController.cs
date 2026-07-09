namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for listing management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/listings")]
public class ListingController : ControllerBase
{
    private readonly IListingService _listingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListingController"/> class.
    /// </summary>
    /// <param name="listingService">The listing service.</param>
    public ListingController(IListingService listingService)
    {
        _listingService = listingService;
    }

    /// <summary>
    /// Gets all listings.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The collection of listings.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var listings = await _listingService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(listings);
    }

    /// <summary>
    /// Gets a listing by its identifier.
    /// </summary>
    /// <param name="id">The listing identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The listing.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.GetByIdAsync(id, cancellationToken);
        return Ok(listing);
    }

    /// <summary>
    /// Creates a new listing.
    /// </summary>
    /// <param name="request">The create request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created listing.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateListingRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = listing.Id }, listing);
    }

    /// <summary>
    /// Updates an existing listing.
    /// </summary>
    /// <param name="id">The listing identifier.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated listing.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateListingRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.UpdateAsync(id, request, cancellationToken);
        return Ok(listing);
    }

    /// <summary>
    /// Deletes a listing.
    /// </summary>
    /// <param name="id">The listing identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _listingService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Approves a listing.
    /// </summary>
    /// <param name="id">The listing identifier.</param>
    /// <param name="request">The verify request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The approved listing.</returns>
    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(
        int id,
        [FromBody] VerifyVendorRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.ApproveAsync(id, request.UpdatedBy, cancellationToken);
        return Ok(listing);
    }

    /// <summary>
    /// Rejects a listing.
    /// </summary>
    /// <param name="id">The listing identifier.</param>
    /// <param name="request">The reject request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rejected listing.</returns>
    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(
        int id,
        [FromBody] RejectVendorRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.RejectAsync(id, request.Reason, request.UpdatedBy, cancellationToken);
        return Ok(listing);
    }

    /// <summary>
    /// Archives a listing.
    /// </summary>
    /// <param name="id">The listing identifier.</param>
    /// <param name="request">The verify request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The archived listing.</returns>
    [HttpPost("{id:int}/archive")]
    public async Task<IActionResult> Archive(
        int id,
        [FromBody] VerifyVendorRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.ArchiveAsync(id, request.UpdatedBy, cancellationToken);
        return Ok(listing);
    }

    /// <summary>
    /// Restores a listing.
    /// </summary>
    /// <param name="id">The listing identifier.</param>
    /// <param name="request">The verify request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The restored listing.</returns>
    [HttpPost("{id:int}/restore")]
    public async Task<IActionResult> Restore(
        int id,
        [FromBody] VerifyVendorRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.RestoreAsync(id, request.UpdatedBy, cancellationToken);
        return Ok(listing);
    }
}
