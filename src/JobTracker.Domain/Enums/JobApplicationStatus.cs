namespace JobTracker.Domain.Enums;

/// <summary>
/// Represents the current stage of a job application.
/// </summary>
public enum JobApplicationStatus
{
    /// <summary>
    /// The application has been submitted but not yet reviewed.
    /// </summary>
    Applied = 0,

    /// <summary>
    /// The application is under initial review or screening.
    /// </summary>
    Screening = 1,

    /// <summary>
    /// The applicant has been invited for an interview.
    /// </summary>
    Interview = 2,

    /// <summary>
    /// The applicant has received a job offer.
    /// </summary>
    Offer = 3,

    /// <summary>
    /// The application has been rejected.
    /// </summary>
    Rejected = 4,
}
