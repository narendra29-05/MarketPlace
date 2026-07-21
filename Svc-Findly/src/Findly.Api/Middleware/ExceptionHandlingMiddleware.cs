using Findly.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Findly.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate                      _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (statusCode, title) = ex switch
            {
                AuthenticationFailedException         => (StatusCodes.Status401Unauthorized,       "Authentication failed."),
                KeyNotFoundException                  => (StatusCodes.Status404NotFound,           "Resource not found."),
                UnauthorizedAccessException           => (StatusCodes.Status403Forbidden,          "Access denied."),
                InvalidOperationException             => (StatusCodes.Status409Conflict,           "Operation not allowed in the current state."),
                ArgumentException                     => (StatusCodes.Status400BadRequest,         "Invalid request."),
                SqlException { Number: 2601 or 2627 } => (StatusCodes.Status409Conflict,           "A record with the same unique value already exists."),
                SqlException { Number: 547 }          => (StatusCodes.Status409Conflict,           "The record is referenced by other records and cannot be modified."),
                _                                     => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status   = statusCode,
                    Title    = title,
                    Detail   = statusCode == StatusCodes.Status500InternalServerError ? null : ex.Message,
                    Instance = context.Request.Path
                },
                options: null,
                contentType: "application/problem+json");
        }
    }
}
