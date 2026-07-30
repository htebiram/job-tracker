using FluentValidation;
using JobTracker.Application.JobApplications.Models;

namespace JobTracker.Application.JobApplications.Validation;

public sealed class JobApplicationQueryValidator
    : AbstractValidator<JobApplicationQuery>
{
    public JobApplicationQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.SortBy).IsInEnum();
        RuleFor(query => query.SortDirection).IsInEnum();
        RuleFor(query => query.DateTo)
            .GreaterThanOrEqualTo(query => query.DateFrom)
            .When(query => query.DateFrom.HasValue && query.DateTo.HasValue);
    }
}
