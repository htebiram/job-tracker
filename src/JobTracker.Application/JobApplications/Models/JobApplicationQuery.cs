using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JobTracker.Domain.Enums;

namespace JobTracker.Application.JobApplications.Models;

public sealed class JobApplicationQuery
{
    [Description("Filters applications by status.")]
    public JobApplicationStatus? Status { get; init; }

    [Description("Filters by a partial company name.")]
    public string? Company { get; init; }

    [Description("Filters by a partial role or job title.")]
    public string? Role { get; init; }

    [Description("Filters by the source where the job was found.")]
    public string? Source { get; init; }

    [DataType(DataType.Date)]
    [Description(
        "Includes applications on or after this date. Format: yyyy-MM-dd. "
        + "Example: 2026-07-30.")]
    public DateTime? DateFrom { get; init; }

    [DataType(DataType.Date)]
    [Description(
        "Includes applications on or before this date. Format: yyyy-MM-dd. "
        + "Example: 2026-07-30.")]
    public DateTime? DateTo { get; init; }

    [Description(
        "Searches company, role, source, location, and notes.")]
    public string? Search { get; init; }

    [Description("Field used for sorting.")]
    public JobApplicationSortBy SortBy { get; init; } =
        JobApplicationSortBy.DateApplied;

    [Description(
        "Sort order. Accepted values: Ascending or Descending. "
        + "Default: Descending.")]
    public SortDirection SortDirection { get; init; } =
        SortDirection.Descending;

    [Description("Page number starting at 1. Default: 1.")]
    public int Page { get; init; } = 1;

    [Description(
        "Number of records returned per page, from 1 to 100. Default: 20.")]
    public int PageSize { get; init; } = 20;
}
