namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing a listing's category and tag classification links (taxonomy).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/listings/{listingId:int}")]
public class ListingClassificationController : ControllerBase
{
    private readonly IListingTaxonomyService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListingClassificationController"/> class.
    /// </summary>
    /// <param name="service">The listing taxonomy service.</param>
    public ListingClassificationController(IListingTaxonomyService service)
    {
        _service = service;
    }

    // ---- Categories ----

    /// <summary>
    /// Gets the category links for a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The listing's category links.</returns>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(int listingId, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetCategoriesAsync(listingId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Adds a category link to a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="request">The add listing category request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created category link.</returns>
    [HttpPost("categories")]
    public async Task<IActionResult> AddCategory(int listingId, [FromBody] AddListingCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _service.AddCategoryAsync(listingId, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Removes a category link from a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="linkId">The category link identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A no content result.</returns>
    [HttpDelete("categories/{linkId:int}")]
    public async Task<IActionResult> RemoveCategory(int listingId, int linkId, CancellationToken cancellationToken = default)
    {
        await _service.RemoveCategoryAsync(linkId, cancellationToken);
        return NoContent();
    }

    // ---- Tags ----

    /// <summary>
    /// Gets the tag links for a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The listing's tag links.</returns>
    [HttpGet("tags")]
    public async Task<IActionResult> GetTags(int listingId, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetTagsAsync(listingId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Adds a tag link to a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="request">The add listing tag request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created tag link.</returns>
    [HttpPost("tags")]
    public async Task<IActionResult> AddTag(int listingId, [FromBody] AddListingTagRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _service.AddTagAsync(listingId, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Removes a tag link from a listing.
    /// </summary>
    /// <param name="listingId">The listing identifier.</param>
    /// <param name="linkId">The tag link identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A no content result.</returns>
    [HttpDelete("tags/{linkId:int}")]
    public async Task<IActionResult> RemoveTag(int listingId, int linkId, CancellationToken cancellationToken = default)
    {
        await _service.RemoveTagAsync(linkId, cancellationToken);
        return NoContent();
    }
}
