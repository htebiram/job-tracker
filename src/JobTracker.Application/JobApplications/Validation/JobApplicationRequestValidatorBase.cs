using FluentValidation;

namespace JobTracker.Application.JobApplications.Validation;

public abstract class JobApplicationRequestValidatorBase<T>
    : AbstractValidator<T>
{
    protected void ConfigureRules(
        Func<T, string> companyName,
        Func<T, string> role,
        Func<T, DateTime> dateApplied,
        Func<T, decimal?> expectedSalary,
        Func<T, string?> link,
        Func<T, string?> source,
        Func<T, string?> location,
        Func<T, string?> notes)
    {
        RuleFor(x => companyName(x))
            .NotEmpty()
            .MaximumLength(200)
            .WithName("CompanyName");
        RuleFor(x => role(x))
            .NotEmpty()
            .MaximumLength(200)
            .WithName("Role");
        RuleFor(x => dateApplied(x))
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(1))
            .WithName("DateApplied");
        RuleFor(x => expectedSalary(x))
            .GreaterThanOrEqualTo(0)
            .When(x => expectedSalary(x).HasValue)
            .WithName("ExpectedSalary");
        RuleFor(x => link(x))
            .MaximumLength(500)
            .Must(value => value is null
                || Uri.TryCreate(value, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp
                    || uri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("Link must be a valid HTTP or HTTPS URL.")
            .WithName("Link");
        RuleFor(x => source(x))
            .MaximumLength(100)
            .WithName("Source");
        RuleFor(x => location(x))
            .MaximumLength(200)
            .WithName("Location");
        RuleFor(x => notes(x))
            .MaximumLength(4000)
            .WithName("Notes");
    }
}
