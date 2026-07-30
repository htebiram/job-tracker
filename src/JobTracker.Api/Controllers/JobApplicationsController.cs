using JobTracker.Application.Common;
using JobTracker.Application.JobApplications.Models;
using JobTracker.Application.JobApplications.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/jobapplications")]
[Produces("application/json")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
[ProducesResponseType<ProblemDetails>(
    StatusCodes.Status500InternalServerError)]
[ProducesResponseType<ProblemDetails>(
    StatusCodes.Status503ServiceUnavailable)]
public sealed class JobApplicationsController(
    IJobApplicationService service) : ControllerBase
{
    /// <summary>
    /// Gets a filtered, sorted, and paged list of job applications.
    /// </summary>
    [HttpGet]
    [EndpointSummary("List job applications")]
    [EndpointDescription(
        "Returns job applications with optional filtering, searching, "
        + "sorting, and pagination.")]
    [ProducesResponseType<PagedResult<JobApplicationDto>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<JobApplicationDto>>> GetAsync(
        [FromQuery] JobApplicationQuery query,
        CancellationToken cancellationToken) =>
        Ok(await service.GetAsync(query, cancellationToken));

    /// <summary>
    /// Gets one job application by its identifier.
    /// </summary>
    [HttpGet("{id}", Name = nameof(GetByIdAsync))]
    [ActionName(nameof(GetByIdAsync))]
    [EndpointSummary("Get a job application")]
    [EndpointDescription(
        "Returns the matching job application, or 404 when it does not exist.")]
    [ProducesResponseType<JobApplicationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobApplicationDto>> GetByIdAsync(
        [FromRoute]
        [Range(1, int.MaxValue)]
        [Description("Unique job application identifier.")]
        int id,
        CancellationToken cancellationToken)
    {
        var application = await service.GetByIdAsync(id, cancellationToken);
        return application is null
            ? JobApplicationNotFound(id)
            : Ok(application);
    }

    /// <summary>
    /// Creates a job application.
    /// </summary>
    [HttpPost]
    [EndpointSummary("Create a job application")]
    [EndpointDescription(
        "Creates a validated job application and returns its canonical URL.")]
    [ProducesResponseType<JobApplicationDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobApplicationDto>> CreateAsync(
        [FromBody]
        [Required]
        [Description("Job application to create.")]
        CreateJobApplicationRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return InvalidRequestBody();
        }

        var application = await service.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = application.Id },
            application);
    }

    /// <summary>
    /// Updates an existing job application.
    /// </summary>
    [HttpPut("{id}")]
    [EndpointSummary("Update a job application")]
    [EndpointDescription(
        "Replaces the editable fields of an existing job application.")]
    [ProducesResponseType<JobApplicationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobApplicationDto>> UpdateAsync(
        [FromRoute]
        [Range(1, int.MaxValue)]
        [Description("Unique job application identifier.")]
        int id,
        [FromBody]
        [Required]
        [Description("Replacement values for the job application.")]
        UpdateJobApplicationRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return InvalidRequestBody();
        }

        var application = await service.UpdateAsync(
            id,
            request,
            cancellationToken);
        return application is null
            ? JobApplicationNotFound(id)
            : Ok(application);
    }

    /// <summary>
    /// Deletes a job application.
    /// </summary>
    [HttpDelete("{id}")]
    [EndpointSummary("Delete a job application")]
    [EndpointDescription(
        "Deletes the matching job application and returns no content.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute]
        [Range(1, int.MaxValue)]
        [Description("Unique job application identifier.")]
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : JobApplicationNotFound(id);
    }

    /// <summary>
    /// Gets application counts grouped by status.
    /// </summary>
    [HttpGet("status-counts")]
    [EndpointSummary("Get job application status counts")]
    [EndpointDescription(
        "Returns the number of job applications in each represented status.")]
    [ProducesResponseType<IReadOnlyList<StatusCountDto>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<StatusCountDto>>>
        GetStatusCountsAsync(CancellationToken cancellationToken) =>
        Ok(await service.GetStatusCountsAsync(cancellationToken));

    private ObjectResult JobApplicationNotFound(int id)
    {
        var result = new ObjectResult(
            ApiProblemDetailsFactory.Create(
                HttpContext,
                StatusCodes.Status404NotFound,
                "Job application not found.",
                $"No job application with identifier {id} was found."))
        {
            StatusCode = StatusCodes.Status404NotFound,
        };
        result.ContentTypes.Add("application/problem+json");
        return result;
    }

    private ObjectResult InvalidRequestBody()
    {
        var result = new ObjectResult(
            ApiProblemDetailsFactory.CreateValidation(
                HttpContext,
                new Dictionary<string, string[]>
                {
                    ["request"] =
                    [
                        "A non-null request body is required.",
                    ],
                }))
        {
            StatusCode = StatusCodes.Status400BadRequest,
        };
        result.ContentTypes.Add("application/problem+json");
        return result;
    }
}
