using System.Data.Common;
using System.Text.Json;
using JobTracker.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace JobTracker.Tests.Unit;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task DatabaseFailure_ReturnsServiceUnavailableAsync()
    {
        var context = CreateContext();
        var exception = new InvalidOperationException(
            "Transient database failure.",
            new TestDbException());

        await CreateHandler().TryHandleAsync(
            context,
            exception,
            CancellationToken.None);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(
            StatusCodes.Status503ServiceUnavailable,
            context.Response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            context.Response.ContentType);
        Assert.Equal("5", context.Response.Headers.RetryAfter);
        Assert.Equal(
            "The database is temporarily unavailable.",
            problem.Title);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, problem.Status);
    }

    [Fact]
    public async Task UnexpectedFailure_ReturnsInternalServerErrorAsync()
    {
        var context = CreateContext();

        await CreateHandler().TryHandleAsync(
            context,
            new InvalidOperationException("Unexpected."),
            CancellationToken.None);

        var problem = await ReadProblemDetailsAsync(context);
        Assert.Equal(
            StatusCodes.Status500InternalServerError,
            context.Response.StatusCode);
        Assert.Equal("An unexpected error occurred.", problem.Title);
        Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
    }

    private static GlobalExceptionHandler CreateHandler() =>
        new(NullLogger<GlobalExceptionHandler>.Instance);

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/jobapplications";
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<ProblemDetails> ReadProblemDetailsAsync(
        HttpContext context)
    {
        context.Response.Body.Position = 0;
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return Assert.IsType<ProblemDetails>(problem);
    }

    private sealed class TestDbException : DbException;
}
