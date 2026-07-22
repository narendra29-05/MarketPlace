using Asp.Versioning;
using Findly.Application.Catalog.Interfaces;
using Findly.Contracts.Catalog.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Findly.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/catalog")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet("listings")]
    public async Task<IActionResult> Search(
        [FromQuery] CatalogSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var listings = await _catalogService.SearchAsync(request, cancellationToken);
        return Ok(listings);
    }

    [HttpGet("listings/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken = default)
    {
        var listing = await _catalogService.GetBySlugAsync(slug, cancellationToken);
        return Ok(listing);
    }

    [HttpGet("listings/{slug}/reviews")]
    public async Task<IActionResult> GetReviews(
        string slug,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var reviews = await _catalogService.GetReviewsAsync(slug, page, pageSize, cancellationToken);
        return Ok(reviews);
    }

    /// <summary>Side-by-side comparison of 2–4 published listings, e.g. /catalog/compare?ids=1,2,3</summary>
    [HttpGet("compare")]
    public async Task<IActionResult> Compare(
        [FromQuery] string ids,
        CancellationToken cancellationToken = default)
    {
        var listingIds = ParseIds(ids);
        var comparison = await _catalogService.CompareAsync(listingIds, cancellationToken);
        return Ok(comparison);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken = default)
    {
        var categories = await _catalogService.GetCategoriesAsync(cancellationToken);
        return Ok(categories);
    }

    private static List<int> ParseIds(string? ids)
    {
        if (string.IsNullOrWhiteSpace(ids))
            throw new ArgumentException("Query parameter 'ids' is required, e.g. ?ids=1,2");

        var result = new List<int>();
        foreach (var part in ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!int.TryParse(part, out var id))
                throw new ArgumentException($"'{part}' is not a valid listing id.");
            result.Add(id);
        }

        return result;
    }
}
