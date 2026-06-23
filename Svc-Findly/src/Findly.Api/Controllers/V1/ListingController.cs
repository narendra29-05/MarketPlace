using Asp.Versioning;
using Findly.Application.Listings.Interfaces;
using Findly.Contracts.Common;
using Findly.Contracts.Listing.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Findly.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/listings")]
public class ListingController : ControllerBase
{
    private readonly IListingService _listingService;

    public ListingController(IListingService listingService)
    {
        _listingService = listingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var listings = await _listingService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(listings);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.GetByIdAsync(id, cancellationToken);
        return Ok(listing);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateListingRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = listing.Id }, listing);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateListingRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.UpdateAsync(id, request, cancellationToken);
        return Ok(listing);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _listingService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(
        int id,
        [FromBody] UpdatedByRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.ApproveAsync(id, request.UpdatedBy, cancellationToken);
        return Ok(listing);
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(
        int id,
        [FromBody] RejectRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.RejectAsync(id, request.Reason, request.UpdatedBy, cancellationToken);
        return Ok(listing);
    }

    [HttpPost("{id:int}/archive")]
    public async Task<IActionResult> Archive(
        int id,
        [FromBody] UpdatedByRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.ArchiveAsync(id, request.UpdatedBy, cancellationToken);
        return Ok(listing);
    }
}
