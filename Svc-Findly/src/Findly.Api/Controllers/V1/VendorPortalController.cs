using Asp.Versioning;
using Findly.Application.Leads.Interfaces;
using Findly.Application.Listings.Interfaces;
using Findly.Application.Vendors.Interfaces;
using Findly.Contracts.Lead.Requests;
using Findly.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Findly.Api.Controllers.V1;

/// <summary>Vendor-facing views over the signed-in vendor's own listings and leads.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/vendor-portal")]
[Authorize(Roles = "Vendor")]
public class VendorPortalController : ControllerBase
{
    private readonly IVendorService  _vendorService;
    private readonly IListingService _listingService;
    private readonly ILeadService    _leadService;

    public VendorPortalController(
        IVendorService  vendorService,
        IListingService listingService,
        ILeadService    leadService)
    {
        _vendorService  = vendorService;
        _listingService = listingService;
        _leadService    = leadService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.GetMyVendorAsync(cancellationToken);
        return Ok(vendor);
    }

    [HttpGet("listings")]
    public async Task<IActionResult> GetListings(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ListingStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var listings = await _listingService.GetMyListingsAsync(page, pageSize, status, cancellationToken);
        return Ok(listings);
    }

    [HttpGet("leads")]
    public async Task<IActionResult> GetLeads(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] LeadStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var leads = await _leadService.GetMyLeadsAsync(status, page, pageSize, cancellationToken);
        return Ok(leads);
    }

    [HttpGet("leads/{id:int}")]
    public async Task<IActionResult> GetLeadById(int id, CancellationToken cancellationToken = default)
    {
        var lead = await _leadService.GetByIdAsync(id, cancellationToken);
        return Ok(lead);
    }

    [HttpPost("leads/{id:int}/status")]
    public async Task<IActionResult> UpdateLeadStatus(
        int id,
        [FromBody] UpdateLeadStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var lead = await _leadService.UpdateStatusAsync(id, request.Status, cancellationToken);
        return Ok(lead);
    }
}
