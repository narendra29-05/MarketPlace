using Asp.Versioning;
using Findly.Application.Leads.Interfaces;
using Findly.Application.Listings.Interfaces;
using Findly.Application.Reviews.Interfaces;
using Findly.Application.Vendors.Interfaces;
using Findly.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Findly.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IVendorService  _vendorService;
    private readonly IListingService _listingService;
    private readonly IReviewService  _reviewService;
    private readonly ILeadService    _leadService;

    public AdminController(
        IVendorService  vendorService,
        IListingService listingService,
        IReviewService  reviewService,
        ILeadService    leadService)
    {
        _vendorService  = vendorService;
        _listingService = listingService;
        _reviewService  = reviewService;
        _leadService    = leadService;
    }

    [HttpGet("vendors/pending")]
    public async Task<IActionResult> GetPendingVendors(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var vendors = await _vendorService.GetAllAsync(page, pageSize, VendorStatus.Pending, cancellationToken);
        return Ok(vendors);
    }

    [HttpGet("listings/pending")]
    public async Task<IActionResult> GetPendingListings(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var listings = await _listingService.GetAllAsync(page, pageSize, ListingStatus.Pending, cancellationToken);
        return Ok(listings);
    }

    [HttpGet("reviews/pending")]
    public async Task<IActionResult> GetPendingReviews(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var reviews = await _reviewService.GetAllAsync(ReviewStatus.Pending, page, pageSize, cancellationToken);
        return Ok(reviews);
    }

    [HttpGet("leads")]
    public async Task<IActionResult> GetLeads(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] LeadStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var leads = await _leadService.GetAllAsync(status, page, pageSize, cancellationToken);
        return Ok(leads);
    }
}
