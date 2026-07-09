namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for lead management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/leads")]
public class LeadController : ControllerBase
{
    private readonly ILeadService _leadService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeadController"/> class.
    /// </summary>
    /// <param name="leadService">The lead service.</param>
    public LeadController(ILeadService leadService)
    {
        _leadService = leadService;
    }

    /// <summary>
    /// Gets a lead by identifier.
    /// </summary>
    /// <param name="id">The lead identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The lead details.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _leadService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets the leads for a specific vendor.
    /// </summary>
    /// <param name="vendorId">The vendor identifier.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paged list of leads for the vendor.</returns>
    [HttpGet("~/api/v{version:apiVersion}/vendors/{vendorId:int}/leads")]
    public async Task<IActionResult> GetByVendor(int vendorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _leadService.GetByVendorAsync(vendorId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new lead.
    /// </summary>
    /// <param name="request">The lead creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created lead.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeadRequest request, CancellationToken cancellationToken = default)
    {
        var lead = await _leadService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = lead.Id }, lead);
    }

    /// <summary>
    /// Updates the status of a lead.
    /// </summary>
    /// <param name="id">The lead identifier.</param>
    /// <param name="request">The status update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated lead.</returns>
    [HttpPost("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateLeadStatusRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _leadService.UpdateStatusAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a lead.
    /// </summary>
    /// <param name="id">The lead identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A no content result.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _leadService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
