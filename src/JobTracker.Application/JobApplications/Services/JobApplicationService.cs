using JobTracker.Application.Common;
using JobTracker.Application.JobApplications.Models;
using JobTracker.Application.JobApplications.Persistence;
using JobTracker.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace JobTracker.Application.JobApplications.Services;

public sealed class JobApplicationService(
    IJobApplicationRepository repository,
    ILogger<JobApplicationService> logger) : IJobApplicationService
{
    private const string AUTHENTICATED_USER_ID = "authenticated";

    public Task<PagedResult<JobApplicationDto>> GetAsync(
        JobApplicationQuery query,
        CancellationToken cancellationToken) =>
        repository.GetAsync(query, cancellationToken);

    public Task<JobApplicationDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<JobApplicationDto> CreateAsync(
        CreateJobApplicationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var now = DateTime.UtcNow;
        var application = new JobApplication
        {
            UserId = AUTHENTICATED_USER_ID,
            CompanyName = request.CompanyName.Trim(),
            Role = request.Role.Trim(),
            Status = request.Status,
            Source = Normalize(request.Source),
            DateApplied = request.DateApplied,
            Location = Normalize(request.Location),
            ExpectedSalary = request.ExpectedSalary,
            Link = Normalize(request.Link),
            Notes = Normalize(request.Notes),
            CreatedAt = now,
            ModifiedAt = now,
        };

        await repository.AddAsync(application, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Created job application {JobApplicationId}.",
            application.Id);

        return Map(application);
    }

    public async Task<JobApplicationDto?> UpdateAsync(
        int id,
        UpdateJobApplicationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var application = await repository.FindByIdAsync(
            id,
            cancellationToken);
        if (application is null)
        {
            return null;
        }

        application.CompanyName = request.CompanyName.Trim();
        application.Role = request.Role.Trim();
        application.Status = request.Status;
        application.Source = Normalize(request.Source);
        application.DateApplied = request.DateApplied;
        application.Location = Normalize(request.Location);
        application.ExpectedSalary = request.ExpectedSalary;
        application.Link = Normalize(request.Link);
        application.Notes = Normalize(request.Notes);
        application.ModifiedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Updated job application {JobApplicationId}.", id);

        return Map(application);
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var application = await repository.FindByIdAsync(
            id,
            cancellationToken);
        if (application is null)
        {
            return false;
        }

        repository.Remove(application);
        await repository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Deleted job application {JobApplicationId}.", id);

        return true;
    }

    public Task<IReadOnlyList<StatusCountDto>> GetStatusCountsAsync(
        CancellationToken cancellationToken) =>
        repository.GetStatusCountsAsync(cancellationToken);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static JobApplicationDto Map(JobApplication application) =>
        new(
            application.Id,
            application.CompanyName,
            application.Role,
            application.Status,
            application.Source,
            application.DateApplied,
            application.Location,
            application.ExpectedSalary,
            application.Link,
            application.Notes,
            application.CreatedAt,
            application.ModifiedAt);
}
