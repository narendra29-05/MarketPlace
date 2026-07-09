namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for category management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryController"/> class.
    /// </summary>
    /// <param name="categoryService">The category service.</param>
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Gets all categories.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paged list of categories.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a category by identifier.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The category details.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="request">The category creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created category.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="request">The category update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated category.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Activates a category.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="request">The request containing the updater.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The activated category.</returns>
    [HttpPost("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id, [FromBody] ApproveReviewRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.ActivateAsync(id, request.UpdatedBy, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deactivates a category.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="request">The request containing the updater.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deactivated category.</returns>
    [HttpPost("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id, [FromBody] ApproveReviewRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.DeactivateAsync(id, request.UpdatedBy, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a category.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _categoryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
