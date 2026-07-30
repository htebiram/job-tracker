using System.Net;
using System.Net.Http.Json;
using JobTracker.Application.Common;
using JobTracker.Application.JobApplications.Models;
using JobTracker.Application.JobApplications.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobTracker.Tests.Integration;

public sealed class ErrorHandlingEndpointsTests
    : IClassFixture<JobTrackerApiFactory>
{
    private readonly JobTrackerApiFactory _factory;

    public ErrorHandlingEndpointsTests(JobTrackerApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UnexpectedServiceFailure_ReturnsSafeProblemDetailsAsync()
    {
        const string sensitiveMessage =
            "Server=secret;Password=do-not-return";
        using var factory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IJobApplicationService>();
                services.AddSingleton<IJobApplicationService>(
                    new ThrowingService(sensitiveMessage));
            }));
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
            });
        client.DefaultRequestHeaders.Add(
            TestAuthenticationHandler.USER_HEADER,
            "auth0|error-test-user");

        var response = await client.GetAsync("/api/jobapplications");

        Assert.Equal(
            HttpStatusCode.InternalServerError,
            response.StatusCode);
        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("An unexpected error occurred.", problem?.Title);
        Assert.Equal(
            "An unexpected error occurred while processing the request.",
            problem?.Detail);
        Assert.Equal("/api/jobapplications", problem?.Instance);
        Assert.DoesNotContain(
            sensitiveMessage,
            await response.Content.ReadAsStringAsync());
        Assert.True(problem?.Extensions.ContainsKey("traceId"));
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsSuccessAsync()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private sealed class ThrowingService(string message)
        : IJobApplicationService
    {
        public Task<PagedResult<JobApplicationDto>> GetAsync(
            JobApplicationQuery query,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException(message);

        public Task<JobApplicationDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException(message);

        public Task<JobApplicationDto> CreateAsync(
            CreateJobApplicationRequest request,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException(message);

        public Task<JobApplicationDto?> UpdateAsync(
            int id,
            UpdateJobApplicationRequest request,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException(message);

        public Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException(message);

        public Task<IReadOnlyList<StatusCountDto>> GetStatusCountsAsync(
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException(message);
    }
}
