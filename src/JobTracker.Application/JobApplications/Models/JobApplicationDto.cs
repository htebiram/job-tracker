using JobTracker.Domain.Enums;

namespace JobTracker.Application.JobApplications.Models;

public sealed record JobApplicationDto(
    int Id,
    string CompanyName,
    string Role,
    JobApplicationStatus Status,
    string? Source,
    DateTime DateApplied,
    string? Location,
    decimal? ExpectedSalary,
    string? Link,
    string? Notes,
    DateTime CreatedAt,
    DateTime ModifiedAt);
