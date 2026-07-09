namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for vendor management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/vendors")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;

    /// <summary>
    /// Initializes a new instance of the <see cref="VendorController"/> class.
    /// </summary>
    /// <param name="vendorService">The vendor service.</param>
    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    /// <summary>
    /// Gets all vendors with pagination.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paged list of vendors.</returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var vendors = await _vendorService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(vendors);
    }

    /// <summary>
    /// Gets a vendor by identifier.
    /// </summary>
    /// <param name="id">The vendor identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The vendor details.</returns>
    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.GetByIdAsync(id, cancellationToken);
        return Ok(vendor);
    }

    /// <summary>
    /// Creates a new vendor.
    /// </summary>
    /// <param name="request">The create request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created vendor.</returns>
    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateVendorRequest request,
        CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = vendor.Id }, vendor);
    }

    /// <summary>
    /// Updates an existing vendor's details.
    /// </summary>
    /// <param name="id">The vendor identifier.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated vendor.</returns>
    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> UpdateDetails(
        int id,
        [FromBody] UpdateVendorRequest request,
        CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.UpdateAsync(id, request, cancellationToken);
        return Ok(vendor);
    }

    /// <summary>
    /// Updates an existing vendor's address.
    /// </summary>
    /// <param name="id">The vendor identifier.</param>
    /// <param name="request">The address update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated vendor.</returns>
    [HttpPut("update-address/{id:int}")]
    public async Task<IActionResult> UpdateAddress(
        int id,
        [FromBody] UpdateVendorAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.UpdateAddressAsync(id, request, cancellationToken);
        return Ok(vendor);
    }

    /// <summary>
    /// Deletes a vendor.
    /// </summary>
    /// <param name="id">The vendor identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content when deleted; otherwise not found.</returns>
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _vendorService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Verifies a pending vendor.
    /// </summary>
    /// <param name="id">The vendor identifier.</param>
    /// <param name="request">The request containing the actor performing the action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The verified vendor.</returns>
    [HttpPut("verify/{id:int}")]
    public async Task<IActionResult> Verify(
        int id,
        [FromBody] VerifyVendorRequest request,
        CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.VerifyAsync(id, request.UpdatedBy, cancellationToken);
        return Ok(vendor);
    }

    /// <summary>
    /// Rejects a pending vendor.
    /// </summary>
    /// <param name="id">The vendor identifier.</param>
    /// <param name="request">The request containing the rejection reason and actor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rejected vendor.</returns>
    [HttpPut("reject/{id:int}")]
    public async Task<IActionResult> Reject(
        int id,
        [FromBody] RejectVendorRequest request,
        CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorService.RejectAsync(id, request.Reason, request.UpdatedBy, cancellationToken);
        return Ok(vendor);
    }
}
