using JobTracker.Application.Common;
using JobTracker.Application.JobApplications.Models;

namespace JobTracker.Application.JobApplications.Services;

public interface IJobApplicationService
{
    public Task<PagedResult<JobApplicationDto>> GetAsync(
        JobApplicationQuery query,
        CancellationToken cancellationToken);

    public Task<JobApplicationDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    public Task<JobApplicationDto> CreateAsync(
        CreateJobApplicationRequest request,
        CancellationToken cancellationToken);

    public Task<JobApplicationDto?> UpdateAsync(
        int id,
        UpdateJobApplicationRequest request,
        CancellationToken cancellationToken);

    public Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken);

    public Task<IReadOnlyList<StatusCountDto>> GetStatusCountsAsync(
        CancellationToken cancellationToken);
}
