namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for listing media management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/listings/{listingId:int}/media")]
public class ListingMediaController : ControllerBase
{
    private readonly IListingMediaService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListingMediaController"/> class.
    /// </summary>
    /// <param name="service">The listing media service.</param>
    public ListingMediaController(IListingMediaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets all media for a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The media for the listing.</returns>
    [HttpGet]
    public async Task<IActionResult> GetByListing(int listingId, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByListingAsync(listingId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Adds media to a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="request">The media to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added media.</returns>
    [HttpPost]
    public async Task<IActionResult> Add(int listingId, [FromBody] AddListingMediaRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _service.AddAsync(listingId, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Updates media for a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="mediaId">The media identifier.</param>
    /// <param name="request">The updated media details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated media.</returns>
    [HttpPut("{mediaId:int}")]
    public async Task<IActionResult> Update(int listingId, int mediaId, [FromBody] UpdateListingMediaRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _service.UpdateAsync(mediaId, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes media from a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="mediaId">The media identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{mediaId:int}")]
    public async Task<IActionResult> Delete(int listingId, int mediaId, CancellationToken cancellationToken = default)
    {
        await _service.DeleteAsync(mediaId, cancellationToken);
        return NoContent();
    }
}
