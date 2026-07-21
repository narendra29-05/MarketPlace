using Asp.Versioning;
using Findly.Application.Listings.Interfaces;
using Findly.Contracts.Common;
using Findly.Contracts.Listing.Requests;
using Findly.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Findly.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/listings")]
[Authorize]
public class ListingController : ControllerBase
{
    private readonly IListingService _listingService;

    public ListingController(IListingService listingService)
    {
        _listingService = listingService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ListingStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var listings = await _listingService.GetAllAsync(page, pageSize, status, cancellationToken);
        return Ok(listings);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.GetByIdAsync(id, cancellationToken);
        return Ok(listing);
    }

    [Authorize(Roles = "Vendor")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateListingRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = listing.Id, version = "1.0" }, listing);
    }

    [Authorize(Roles = "Vendor,Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateListingRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.UpdateAsync(id, request, cancellationToken);
        return Ok(listing);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _listingService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.ApproveAsync(id, cancellationToken);
        return Ok(listing);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(
        int id,
        [FromBody] RejectRequest request,
        CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.RejectAsync(id, request.Reason, cancellationToken);
        return Ok(listing);
    }

    [Authorize(Roles = "Vendor,Admin")]
    [HttpPost("{id:int}/archive")]
    public async Task<IActionResult> Archive(int id, CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.ArchiveAsync(id, cancellationToken);
        return Ok(listing);
    }

    [Authorize(Roles = "Vendor,Admin")]
    [HttpPost("{id:int}/restore")]
    public async Task<IActionResult> Restore(int id, CancellationToken cancellationToken = default)
    {
        var listing = await _listingService.RestoreAsync(id, cancellationToken);
        return Ok(listing);
    }
}
