using JobTracker.Application.Common;
using JobTracker.Application.JobApplications.Models;
using JobTracker.Application.JobApplications.Persistence;
using JobTracker.Application.JobApplications.Services;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;

namespace JobTracker.Tests.Unit;

public sealed class JobApplicationServiceTests
{
    [Fact]
    public async Task Create_NormalizesValuesAndPersistsEntityAsync()
    {
        var repository = new FakeRepository();
        var service = CreateService(repository);
        using var cancellationSource = new CancellationTokenSource();
        var request = CreateRequest() with
        {
            CompanyName = "  Contoso  ",
            Role = "  Developer  ",
            Source = "   ",
            Location = "  Remote  ",
            Notes = "\t",
        };

        var result = await service.CreateAsync(
            request,
            cancellationSource.Token);

        Assert.NotNull(repository.Added);
        Assert.Equal("authenticated", repository.Added.UserId);
        Assert.Equal("Contoso", repository.Added.CompanyName);
        Assert.Equal("Developer", repository.Added.Role);
        Assert.Null(repository.Added.Source);
        Assert.Equal("Remote", repository.Added.Location);
        Assert.Null(repository.Added.Notes);
        Assert.Equal(42, result.Id);
        Assert.Equal(
            cancellationSource.Token,
            repository.AddCancellationToken);
        Assert.Equal(
            cancellationSource.Token,
            repository.SaveCancellationToken);
        Assert.Equal(repository.Added.CreatedAt, repository.Added.ModifiedAt);
    }

    [Fact]
    public async Task Update_MissingEntityDoesNotSaveAsync()
    {
        var repository = new FakeRepository();
        var service = CreateService(repository);

        var result = await service.UpdateAsync(
            99,
            UpdateRequest(),
            CancellationToken.None);

        Assert.Null(result);
        Assert.False(repository.SaveCalled);
    }

    [Fact]
    public async Task Update_ExistingEntityChangesEditableValuesAsync()
    {
        var originalModifiedAt = DateTime.UtcNow.AddDays(-2);
        var entity = new JobApplication
        {
            Id = 7,
            UserId = "authenticated",
            CompanyName = "Old",
            Role = "Old",
            DateApplied = DateTime.UtcNow.AddDays(-5),
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            ModifiedAt = originalModifiedAt,
        };
        var repository = new FakeRepository
        {
            FindResult = entity,
        };
        var service = CreateService(repository);

        var result = await service.UpdateAsync(
            entity.Id,
            UpdateRequest() with
            {
                CompanyName = "  New Company ",
                Role = " New Role ",
                Source = " ",
            },
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("New Company", entity.CompanyName);
        Assert.Equal("New Role", entity.Role);
        Assert.Null(entity.Source);
        Assert.True(entity.ModifiedAt > originalModifiedAt);
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public async Task Delete_ExistingAndMissingEntitiesReturnExpectedResultsAsync()
    {
        var entity = new JobApplication
        {
            Id = 8,
            CompanyName = "Company",
            Role = "Role",
        };
        var repository = new FakeRepository
        {
            FindResult = entity,
        };
        var service = CreateService(repository);

        var deleted = await service.DeleteAsync(8, CancellationToken.None);
        repository.FindResult = null;
        var missing = await service.DeleteAsync(9, CancellationToken.None);

        Assert.True(deleted);
        Assert.False(missing);
        Assert.Same(entity, repository.Removed);
        Assert.True(repository.SaveCalled);
    }

    private static JobApplicationService CreateService(
        IJobApplicationRepository repository) =>
        new(repository, NullLogger<JobApplicationService>.Instance);

    private static CreateJobApplicationRequest CreateRequest() =>
        new(
            "Company",
            "Role",
            JobApplicationStatus.Applied,
            "Source",
            DateTime.UtcNow.AddDays(-1),
            null,
            null,
            null,
            null);

    private static UpdateJobApplicationRequest UpdateRequest() =>
        new(
            "Company",
            "Role",
            JobApplicationStatus.Interview,
            "Source",
            DateTime.UtcNow.AddDays(-1),
            null,
            null,
            null,
            null);

    private sealed class FakeRepository : IJobApplicationRepository
    {
        public JobApplication? Added { get; private set; }

        public JobApplication? FindResult { get; set; }

        public JobApplication? Removed { get; private set; }

        public bool SaveCalled { get; private set; }

        public CancellationToken AddCancellationToken { get; private set; }

        public CancellationToken SaveCancellationToken { get; private set; }

        public Task<PagedResult<JobApplicationDto>> GetAsync(
            JobApplicationQuery query,
            CancellationToken cancellationToken) =>
            Task.FromResult(
                new PagedResult<JobApplicationDto>(
                    [],
                    query.Page,
                    query.PageSize,
                    0));

        public Task<JobApplicationDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken) =>
            Task.FromResult<JobApplicationDto?>(null);

        public Task<JobApplication?> FindByIdAsync(
            int id,
            CancellationToken cancellationToken) =>
            Task.FromResult(FindResult);

        public Task AddAsync(
            JobApplication application,
            CancellationToken cancellationToken)
        {
            application.Id = 42;
            Added = application;
            AddCancellationToken = cancellationToken;
            return Task.CompletedTask;
        }

        public void Remove(JobApplication application)
        {
            Removed = application;
        }

        public Task<IReadOnlyList<StatusCountDto>> GetStatusCountsAsync(
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<StatusCountDto>>([]);

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveCalled = true;
            SaveCancellationToken = cancellationToken;
            return Task.CompletedTask;
        }
    }
}
