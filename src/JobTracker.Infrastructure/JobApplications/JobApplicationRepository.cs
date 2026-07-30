using System.Linq.Expressions;
using JobTracker.Application.Common;
using JobTracker.Application.JobApplications.Models;
using JobTracker.Application.JobApplications.Persistence;
using JobTracker.Domain.Entities;
using JobTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.JobApplications;

public sealed class JobApplicationRepository(
    JobTrackerDbContext dbContext) : IJobApplicationRepository
{
    private static readonly IReadOnlyDictionary<
        JobApplicationSortBy,
        Expression<Func<JobApplication, object>>> _sortExpressions =
        new Dictionary<
            JobApplicationSortBy,
            Expression<Func<JobApplication, object>>>
        {
            [JobApplicationSortBy.DateApplied] =
                application => application.DateApplied,
            [JobApplicationSortBy.CompanyName] =
                application => application.CompanyName,
            [JobApplicationSortBy.Role] =
                application => application.Role,
            [JobApplicationSortBy.Status] =
                application => application.Status,
        };

    public async Task<PagedResult<JobApplicationDto>> GetAsync(
        JobApplicationQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var applications = ApplyFilters(
            dbContext.JobApplications.AsNoTracking(),
            query);
        var totalItems = await applications.CountAsync(cancellationToken);
        applications = ApplySorting(applications, query);

        var items = await Project(applications)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<JobApplicationDto>(
            items,
            query.Page,
            query.PageSize,
            totalItems);
    }

    public Task<JobApplicationDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken) =>
        Project(dbContext.JobApplications
                .AsNoTracking()
                .Where(application => application.Id == id))
            .SingleOrDefaultAsync(cancellationToken);

    public Task<JobApplication?> FindByIdAsync(
        int id,
        CancellationToken cancellationToken) =>
        dbContext.JobApplications.SingleOrDefaultAsync(
            application => application.Id == id,
            cancellationToken);

    public Task AddAsync(
        JobApplication application,
        CancellationToken cancellationToken) =>
        dbContext.JobApplications.AddAsync(
            application,
            cancellationToken).AsTask();

    public void Remove(JobApplication application) =>
        dbContext.JobApplications.Remove(application);

    public async Task<IReadOnlyList<StatusCountDto>> GetStatusCountsAsync(
        CancellationToken cancellationToken)
    {
        var counts = await dbContext.JobApplications
            .AsNoTracking()
            .GroupBy(application => application.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count(),
            })
            .ToListAsync(cancellationToken);

        return counts
            .OrderBy(item => item.Status)
            .Select(item => new StatusCountDto(item.Status, item.Count))
            .ToList();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<JobApplication> ApplyFilters(
        IQueryable<JobApplication> applications,
        JobApplicationQuery query)
    {
        if (query.Status.HasValue)
        {
            applications = applications.Where(
                application => application.Status == query.Status);
        }

        if (!string.IsNullOrWhiteSpace(query.Company))
        {
            applications = applications.Where(application =>
                application.CompanyName.Contains(query.Company));
        }

        if (!string.IsNullOrWhiteSpace(query.Role))
        {
            applications = applications.Where(application =>
                application.Role.Contains(query.Role));
        }

        if (!string.IsNullOrWhiteSpace(query.Source))
        {
            applications = applications.Where(application =>
                application.Source != null
                && application.Source.Contains(query.Source));
        }

        if (query.DateFrom.HasValue)
        {
            applications = applications.Where(application =>
                application.DateApplied >= query.DateFrom);
        }

        if (query.DateTo.HasValue)
        {
            var exclusiveDateTo = query.DateTo.Value.Date.AddDays(1);
            applications = applications.Where(application =>
                application.DateApplied < exclusiveDateTo);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            applications = applications.Where(application =>
                application.CompanyName.Contains(query.Search)
                || application.Role.Contains(query.Search)
                || application.Source != null
                && application.Source.Contains(query.Search)
                || application.Location != null
                && application.Location.Contains(query.Search)
                || application.Notes != null
                && application.Notes.Contains(query.Search));
        }

        return applications;
    }

    private static IQueryable<JobApplication> ApplySorting(
        IQueryable<JobApplication> applications,
        JobApplicationQuery query)
    {
        var expression = _sortExpressions[query.SortBy];

        return query.SortDirection == SortDirection.Descending
            ? applications.OrderByDescending(expression)
            : applications.OrderBy(expression);
    }

    private static IQueryable<JobApplicationDto> Project(
        IQueryable<JobApplication> applications) =>
        applications.Select(application => new JobApplicationDto(
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
            application.ModifiedAt));
}
