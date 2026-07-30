using JobTracker.Application.JobApplications.Models;
using JobTracker.Application.JobApplications.Validation;
using JobTracker.Domain.Enums;

namespace JobTracker.Tests.Unit;

public sealed class JobApplicationValidationTests
{
    [Fact]
    public async Task CreateValidator_RejectsInvalidFieldsAsync()
    {
        var request = new CreateJobApplicationRequest(
            string.Empty,
            string.Empty,
            JobApplicationStatus.Applied,
            null,
            DateTime.UtcNow.AddDays(1),
            null,
            -1,
            "not-a-url",
            null);

        var result = await new CreateJobApplicationRequestValidator()
            .ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            error => error.PropertyName == "CompanyName");
        Assert.Contains(result.Errors,
            error => error.PropertyName == "Role");
        Assert.Contains(result.Errors,
            error => error.PropertyName == "DateApplied");
        Assert.Contains(result.Errors,
            error => error.PropertyName == "ExpectedSalary");
        Assert.Contains(result.Errors,
            error => error.PropertyName == "Link");
    }

    [Fact]
    public async Task QueryValidator_RejectsInvalidPagingAndSortingAsync()
    {
        var query = new JobApplicationQuery
        {
            Page = 0,
            PageSize = 101,
            SortBy = (JobApplicationSortBy)999,
            DateFrom = DateTime.UtcNow,
            DateTo = DateTime.UtcNow.AddDays(-1),
        };

        var result = await new JobApplicationQueryValidator()
            .ValidateAsync(query);

        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
    }

    [Theory]
    [InlineData("CompanyName", 201)]
    [InlineData("Role", 201)]
    [InlineData("Source", 101)]
    [InlineData("Location", 201)]
    [InlineData("Notes", 4001)]
    public async Task CreateValidator_RejectsValuesAboveMaximumLengthAsync(
        string property,
        int length)
    {
        var value = new string('x', length);
        var request = ValidCreate() with
        {
            CompanyName = property == "CompanyName"
                ? value
                : "Company",
            Role = property == "Role" ? value : "Role",
            Source = property == "Source" ? value : null,
            Location = property == "Location" ? value : null,
            Notes = property == "Notes" ? value : null,
        };

        var result = await new CreateJobApplicationRequestValidator()
            .ValidateAsync(request);

        Assert.Contains(
            result.Errors,
            error => error.PropertyName == property);
    }

    [Fact]
    public async Task CreateValidator_AcceptsBoundaryValuesAsync()
    {
        var request = ValidCreate() with
        {
            CompanyName = new string('c', 200),
            Role = new string('r', 200),
            Source = new string('s', 100),
            Location = new string('l', 200),
            Notes = new string('n', 4000),
            ExpectedSalary = 0,
            Link = "https://example.com",
        };

        var result = await new CreateJobApplicationRequestValidator()
            .ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateValidator_RejectsWhitespaceAndInvalidEnumAsync()
    {
        var request = new UpdateJobApplicationRequest(
            "   ",
            "\t",
            (JobApplicationStatus)999,
            null,
            DateTime.UtcNow.AddMinutes(-1),
            null,
            null,
            null,
            null);

        var result = await new UpdateJobApplicationRequestValidator()
            .ValidateAsync(request);

        Assert.Contains(
            result.Errors,
            error => error.PropertyName == "CompanyName");
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == "Role");
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == "Status");
    }

    [Fact]
    public async Task QueryValidator_RejectsNegativeValuesAndSortDirectionAsync()
    {
        var query = new JobApplicationQuery
        {
            Page = -1,
            PageSize = -1,
            SortDirection = (SortDirection)999,
        };

        var result = await new JobApplicationQueryValidator()
            .ValidateAsync(query);

        Assert.Contains(result.Errors, error => error.PropertyName == "Page");
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == "PageSize");
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == "SortDirection");
    }

    private static CreateJobApplicationRequest ValidCreate() =>
        new(
            "Company",
            "Role",
            JobApplicationStatus.Applied,
            null,
            DateTime.UtcNow.AddMinutes(-1),
            null,
            null,
            null,
            null);
}
