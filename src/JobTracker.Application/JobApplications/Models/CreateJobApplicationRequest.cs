using System.ComponentModel;
using JobTracker.Domain.Enums;

namespace JobTracker.Application.JobApplications.Models;

public sealed record CreateJobApplicationRequest(
    [property: DefaultValue("Contoso Ltd.")]
    string CompanyName,
    [property: DefaultValue("Senior .NET Developer")]
    string Role,
    [property: DefaultValue(JobApplicationStatus.Applied)]
    JobApplicationStatus Status,
    [property: DefaultValue("LinkedIn")]
    string? Source,
    [property: Description(
        "Application date. Format: yyyy-MM-dd. Example: 2026-07-30.")]
    DateTime DateApplied,
    [property: DefaultValue("Makati City / Hybrid")]
    string? Location,
    decimal? ExpectedSalary,
    [property: DefaultValue("https://example.com/jobs/senior-dotnet-developer")]
    string? Link,
    [property: DefaultValue("Submitted through the company careers page.")]
    string? Notes);
