using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Findly.Api.Filters;

/// <summary>
/// Translates unhandled exceptions into clean RFC 7807 ProblemDetails responses
/// with the correct HTTP status code. Prevents stack traces and raw SQL errors
/// from leaking to clients.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    // SQL Server error numbers
    private const int SqlUniqueViolation      = 2627; // PK / UNIQUE constraint
    private const int SqlUniqueIndexViolation = 2601; // unique index
    private const int SqlForeignKeyViolation  = 547;  // FK / CHECK constraint

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail) = Map(exception);

        if (status >= StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            _logger.LogWarning("Request failed ({Status}): {Message}", status, exception.Message);

        var problem = new ProblemDetails
        {
            Status = status,
            Title  = title,
            Detail = detail,
            Type   = $"https://httpstatuses.io/{status}"
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static (int status, string title, string detail) Map(Exception exception) => exception switch
    {
        KeyNotFoundException => (
            StatusCodes.Status404NotFound,
            "Resource not found",
            exception.Message),

        InvalidOperationException => (
            StatusCodes.Status409Conflict,
            "Operation not allowed",
            exception.Message),

        ArgumentException => (
            StatusCodes.Status400BadRequest,
            "Invalid request",
            exception.Message),

        SqlException sql when sql.Number is SqlUniqueViolation or SqlUniqueIndexViolation => (
            StatusCodes.Status409Conflict,
            "Conflict",
            "A record with the same unique value already exists."),

        SqlException sql when sql.Number == SqlForeignKeyViolation => (
            StatusCodes.Status409Conflict,
            "Conflict",
            "The operation conflicts with related records and cannot be completed."),

        _ => (
            StatusCodes.Status500InternalServerError,
            "An unexpected error occurred",
            "An unexpected error occurred while processing your request.")
    };
}
