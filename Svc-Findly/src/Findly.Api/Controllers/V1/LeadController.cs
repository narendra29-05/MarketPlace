using Asp.Versioning;
using Findly.Application.Leads.Interfaces;
using Findly.Contracts.Lead.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Findly.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
public class LeadController : ControllerBase
{
    private readonly ILeadService _leadService;

    public LeadController(ILeadService leadService)
    {
        _leadService = leadService;
    }

    /// <summary>Submit a lead for a published listing. Anonymous submission is allowed to maximize conversion.</summary>
    [HttpPost("api/v{version:apiVersion}/listings/{listingId:int}/leads")]
    public async Task<IActionResult> Submit(
        int listingId,
        [FromBody] CreateLeadRequest request,
        CancellationToken cancellationToken = default)
    {
        var lead = await _leadService.SubmitAsync(listingId, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, lead);
    }
}
