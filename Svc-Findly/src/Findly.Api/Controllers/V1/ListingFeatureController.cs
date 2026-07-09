namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for listing feature management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/listings/{listingId:int}/features")]
public class ListingFeatureController : ControllerBase
{
    private readonly IListingFeatureService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListingFeatureController"/> class.
    /// </summary>
    /// <param name="service">The listing feature service.</param>
    public ListingFeatureController(IListingFeatureService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets the features for a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The listing features.</returns>
    [HttpGet]
    public async Task<IActionResult> GetByListing(int listingId, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByListingAsync(listingId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Adds a feature to a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="request">The add listing feature request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created listing feature.</returns>
    [HttpPost]
    public async Task<IActionResult> Add(int listingId, [FromBody] AddListingFeatureRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _service.AddAsync(listingId, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Updates a listing feature.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="featureId">The feature identifier.</param>
    /// <param name="request">The update listing feature request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated listing feature.</returns>
    [HttpPut("{featureId:int}")]
    public async Task<IActionResult> Update(int listingId, int featureId, [FromBody] UpdateListingFeatureRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _service.UpdateAsync(featureId, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a listing feature.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="featureId">The feature identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A no content result.</returns>
    [HttpDelete("{featureId:int}")]
    public async Task<IActionResult> Delete(int listingId, int featureId, CancellationToken cancellationToken = default)
    {
        await _service.DeleteAsync(featureId, cancellationToken);
        return NoContent();
    }
}
