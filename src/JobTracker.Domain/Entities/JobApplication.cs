using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities;

/// <summary>
/// Represents a job application tracked by a user.
/// </summary>
public sealed class JobApplication
{
    /// <summary>
    /// Gets or sets the primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the authenticated user's identifier.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the job title or role.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the application status.
    /// </summary>
    public JobApplicationStatus Status { get; set; }
        = JobApplicationStatus.Applied;

    /// <summary>
    /// Gets or sets where the job was found.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the application date.
    /// </summary>
    public DateTime DateApplied { get; set; }
        = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the job location.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the expected salary.
    /// </summary>
    public decimal? ExpectedSalary { get; set; }

    /// <summary>
    /// Gets or sets the job posting URL.
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// Gets or sets additional notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when the entity was last modified.
    /// </summary>
    public DateTime ModifiedAt { get; set; }
        = DateTime.UtcNow;
}
