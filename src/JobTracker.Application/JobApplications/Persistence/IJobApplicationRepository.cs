using JobTracker.Application.Common;
using JobTracker.Application.JobApplications.Models;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.JobApplications.Persistence;

public interface IJobApplicationRepository
{
    public Task<PagedResult<JobApplicationDto>> GetAsync(
        JobApplicationQuery query,
        CancellationToken cancellationToken);

    public Task<JobApplicationDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    public Task<JobApplication?> FindByIdAsync(
        int id,
        CancellationToken cancellationToken);

    public Task AddAsync(
        JobApplication application,
        CancellationToken cancellationToken);

    public void Remove(JobApplication application);

    public Task<IReadOnlyList<StatusCountDto>> GetStatusCountsAsync(
        CancellationToken cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken);
}
