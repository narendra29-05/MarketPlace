namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for bookmark management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/bookmarks")]
public class BookmarkController : ControllerBase
{
    private readonly IBookmarkService _bookmarkService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookmarkController"/> class.
    /// </summary>
    /// <param name="bookmarkService">The bookmark service.</param>
    public BookmarkController(IBookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    /// <summary>
    /// Gets the bookmarks for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user's bookmarks.</returns>
    [HttpGet("~/api/v{version:apiVersion}/users/{userId:int}/bookmarks")]
    public async Task<IActionResult> GetByUser(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _bookmarkService.GetByUserAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new bookmark.
    /// </summary>
    /// <param name="request">The bookmark creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created bookmark.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookmarkRequest request, CancellationToken cancellationToken = default)
    {
        var bookmark = await _bookmarkService.CreateAsync(request, cancellationToken);
        return Ok(bookmark);
    }

    /// <summary>
    /// Deletes a bookmark by identifier.
    /// </summary>
    /// <param name="id">The bookmark identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content when the bookmark is deleted.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _bookmarkService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
