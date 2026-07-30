using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JobTracker.Api.Middleware;

public static class ApiProblemDetailsFactory
{
    public static ProblemDetails Create(
        HttpContext context,
        int status,
        string title,
        string detail)
    {
        ArgumentNullException.ThrowIfNull(context);

        var problem = new ProblemDetails
        {
            Type = GetTypeUri(status),
            Title = title,
            Status = status,
            Detail = detail,
            Instance = context.Request.Path,
        };
        AddExtensions(problem, context);
        return problem;
    }

    public static ValidationProblemDetails CreateValidation(
        HttpContext context,
        ModelStateDictionary modelState)
    {
        ArgumentNullException.ThrowIfNull(modelState);

        var errors = modelState
            .Where(entry => entry.Value?.ValidationState
                == ModelValidationState.Invalid)
            .ToDictionary(
                entry => NormalizeKey(entry.Key),
                entry => GetMessages(entry.Key, entry.Value!),
                StringComparer.OrdinalIgnoreCase);

        return CreateValidation(context, errors);
    }

    public static ValidationProblemDetails CreateValidation(
        HttpContext context,
        IDictionary<string, string[]> errors)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(errors);

        var problem = new ValidationProblemDetails(errors)
        {
            Type = GetTypeUri(StatusCodes.Status400BadRequest),
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Review the errors property for details.",
            Instance = context.Request.Path,
        };
        AddExtensions(problem, context);
        return problem;
    }

    public static async Task WriteAsync(
        HttpContext context,
        ProblemDetails problem)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(problem);

        context.Response.StatusCode = problem.Status
            ?? StatusCodes.Status500InternalServerError;
        await context.RequestServices
            .GetRequiredService<IProblemDetailsService>()
            .WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = problem,
            });
    }

    private static string[] GetMessages(
        string key,
        ModelStateEntry entry)
    {
        if (IsProperty(key, "status"))
        {
            return
            [
                "Status must be one of: Applied, Screening, Interview, "
                + "Offer, Rejected.",
            ];
        }

        if (IsProperty(key, "sortBy"))
        {
            return
            [
                "SortBy must be one of: DateApplied, CompanyName, Role, "
                + "Status.",
            ];
        }

        if (IsProperty(key, "sortDirection"))
        {
            return
            [
                "SortDirection must be one of: Ascending, Descending.",
            ];
        }

        if (entry.Errors.Any(error => error.Exception is not null))
        {
            return key is "$" or ""
                ? ["The request body contains invalid JSON."]
                : [$"The value supplied for {NormalizeKey(key)} is invalid."];
        }

        var messages = entry.Errors
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToArray();
        return messages.Length > 0
            ? messages
            : [$"The value supplied for {NormalizeKey(key)} is invalid."];
    }

    private static bool IsProperty(string key, string propertyName) =>
        string.Equals(
            NormalizeKey(key),
            propertyName,
            StringComparison.OrdinalIgnoreCase);

    private static string NormalizeKey(string key) =>
        key.StartsWith("$.", StringComparison.Ordinal)
            ? key[2..]
            : string.IsNullOrWhiteSpace(key)
                ? "request"
                : key;

    private static void AddExtensions(
        ProblemDetails problem,
        HttpContext context)
    {
        problem.Extensions["traceId"] = context.TraceIdentifier;
        problem.Extensions["timestamp"] = DateTimeOffset.UtcNow;
    }

    private static string GetTypeUri(int status) =>
        status switch
        {
            StatusCodes.Status400BadRequest =>
                "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.1",
            StatusCodes.Status401Unauthorized =>
                "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.2",
            StatusCodes.Status403Forbidden =>
                "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.4",
            StatusCodes.Status404NotFound =>
                "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.5",
            StatusCodes.Status503ServiceUnavailable =>
                "https://www.rfc-editor.org/rfc/rfc9110#section-15.6.4",
            _ =>
                "https://www.rfc-editor.org/rfc/rfc9110#section-15.6.1",
        };
}
