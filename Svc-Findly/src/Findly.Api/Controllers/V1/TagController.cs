namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for tag management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tags")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagController"/> class.
    /// </summary>
    /// <param name="tagService">The tag service.</param>
    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    /// <summary>
    /// Gets all tags.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paged list of tags.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _tagService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a tag by its identifier.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The requested tag.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _tagService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new tag.
    /// </summary>
    /// <param name="request">The create request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created tag.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request, CancellationToken cancellationToken = default)
    {
        var tag = await _tagService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
    }

    /// <summary>
    /// Updates an existing tag.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated tag.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTagRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _tagService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a tag.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A no content result.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _tagService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
