using System.Data.Common;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Middleware;
/// <summary>
/// Represents a global exception handler that logs unhandled exceptions.
/// </summary>
public sealed class GlobalExceptionHandler
    : IExceptionHandler
{

    /// <summary>
    /// The logger used to log unhandled exceptions.
    /// </summary>
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
    /// </summary>
    /// <param name="logger"></param>
    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// Attempts to handle an unhandled exception by logging it.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(exception);

        var isDatabaseFailure = IsDatabaseFailure(exception);
        var statusCode = isDatabaseFailure
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status500InternalServerError;

        if (isDatabaseFailure)
        {
            _logger.LogError(
                exception,
                "A database operation failed for {RequestPath}.",
                context.Request.Path);
            context.Response.Headers.RetryAfter = "5";
        }
        else
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred for {RequestPath}.",
                context.Request.Path);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        var problemDetails = ApiProblemDetailsFactory.Create(
            context,
            statusCode,
            isDatabaseFailure
                ? "The database is temporarily unavailable."
                : "An unexpected error occurred.",
            isDatabaseFailure
                ? "The request could not be completed because the data "
                    + "store is unavailable. Try again shortly."
                : "An unexpected error occurred while processing the request.");

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            problemDetails,
            cancellationToken: cancellationToken);

        return true;
    }

    private static bool IsDatabaseFailure(Exception exception)
    {
        for (var current = exception;
             current is not null;
             current = current.InnerException)
        {
            if (current is DbException or DbUpdateException)
            {
                return true;
            }
        }

        return false;
    }
}
