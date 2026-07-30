using FluentValidation;
using JobTracker.Application.JobApplications.Models;

namespace JobTracker.Application.JobApplications.Validation;

public sealed class CreateJobApplicationRequestValidator
    : JobApplicationRequestValidatorBase<CreateJobApplicationRequest>
{
    public CreateJobApplicationRequestValidator()
    {
        ConfigureRules(
            request => request.CompanyName,
            request => request.Role,
            request => request.DateApplied,
            request => request.ExpectedSalary,
            request => request.Link,
            request => request.Source,
            request => request.Location,
            request => request.Notes);
        RuleFor(request => request.Status).IsInEnum();
    }
}
