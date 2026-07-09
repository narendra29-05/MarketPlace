namespace Findly.Api.Controllers.V1;

using Asp.Versioning;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for user management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gets a paged list of users.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paged list of users.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a user by identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user details.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The user creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="request">The user update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated user.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Changes the password for a user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="request">The change password request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the password change.</returns>
    [HttpPost("{id:int}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.ChangePasswordAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Verifies the email address for a user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="request">The request containing the updater information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the email verification.</returns>
    [HttpPost("{id:int}/verify-email")]
    public async Task<IActionResult> VerifyEmail(int id, [FromBody] ApproveReviewRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.VerifyEmailAsync(id, request.UpdatedBy, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A no content result.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
