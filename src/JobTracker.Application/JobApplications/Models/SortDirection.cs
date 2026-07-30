namespace JobTracker.Application.JobApplications.Models;

/// <summary>
/// Direction used when sorting results.
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Sort from the lowest value to the highest value.
    /// </summary>
    Ascending = 0,

    /// <summary>
    /// Sort from the highest value to the lowest value.
    /// </summary>
    Descending = 1,
}
