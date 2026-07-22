using Asp.Versioning;
using Findly.Application.Vendors.Interfaces;
using Findly.Contracts.Common;
using Findly.Contracts.Vendor.Requests;
using Findly.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Findly.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/vendors")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] VendorStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var vendors = await _vendorService.GetAllAsync(page, pageSize, status, cancellationToken);
        return Ok(vendors);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.GetByIdAsync(id, cancellationToken);
        return Ok(vendor);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVendorRequest request,
        CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = vendor.Id, version = "1.0" }, vendor);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateVendorRequest request,
        CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.UpdateAsync(id, request, cancellationToken);
        return Ok(vendor);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _vendorService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpPost("{id:int}/verify")]
    public async Task<IActionResult> Verify(int id, CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.VerifyAsync(id, cancellationToken);
        return Ok(vendor);
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(
        int id,
        [FromBody] RejectRequest request,
        CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.RejectAsync(id, request.Reason, cancellationToken);
        return Ok(vendor);
    }
}
