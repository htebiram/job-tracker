using JobTracker.Domain.Enums;

namespace JobTracker.Application.JobApplications.Models;

public sealed record StatusCountDto(
    JobApplicationStatus Status,
    int Count);
