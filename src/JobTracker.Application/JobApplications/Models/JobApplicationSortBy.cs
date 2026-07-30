namespace JobTracker.Application.JobApplications.Models;

/// <summary>
/// Fields supported when sorting job applications.
/// </summary>
public enum JobApplicationSortBy
{
    /// <summary>
    /// Sort by the date the application was submitted.
    /// </summary>
    DateApplied = 0,

    /// <summary>
    /// Sort alphabetically by company name.
    /// </summary>
    CompanyName = 1,

    /// <summary>
    /// Sort alphabetically by job role.
    /// </summary>
    Role = 2,

    /// <summary>
    /// Sort by application status.
    /// </summary>
    Status = 3,
}
