using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using JobTracker.Application.Common;
using JobTracker.Application.JobApplications.Models;
using JobTracker.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JobTracker.Tests.Integration;

public sealed class JobApplicationsEndpointsTests
    : IClassFixture<JobTrackerApiFactory>
{
    private static readonly JsonSerializerOptions _jsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() },
        };

    private readonly HttpClient _client;
    private readonly JobTrackerApiFactory _factory;

    public JobApplicationsEndpointsTests(JobTrackerApiFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });
        _client.DefaultRequestHeaders.Add(
            TestAuthenticationHandler.USER_HEADER,
            "auth0|integration-test-user");
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutAuthentication_ReturnsUnauthorizedAsync()
    {
        using var bearerFactory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })));
        using var anonymousClient = bearerFactory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
            });

        var response = await anonymousClient.GetAsync(
            "/api/jobapplications");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Authentication required", problem?.Title);
        Assert.Equal(
            "A valid access token is required to access this resource.",
            problem?.Detail);
        Assert.Equal("/api/jobapplications", problem?.Instance);
        Assert.True(problem?.Extensions.ContainsKey("traceId"));
    }

    [Fact]
    public void Auth0JwtBearerOptions_EnableRequiredTokenValidation()
    {
        var options = _factory.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.True(options.RequireHttpsMetadata);
        Assert.False(options.MapInboundClaims);
        Assert.Equal(
            "https://jobtracker-test.auth0.com/",
            options.Authority);
        Assert.Equal("https://jobtracker.test/api", options.Audience);
        Assert.True(options.TokenValidationParameters.ValidateIssuer);
        Assert.True(options.TokenValidationParameters.ValidateAudience);
        Assert.True(options.TokenValidationParameters.ValidateLifetime);
        Assert.True(
            options.TokenValidationParameters.ValidateIssuerSigningKey);
        Assert.Equal(
            "sub",
            options.TokenValidationParameters.NameClaimType);
    }

    [Fact]
    public async Task CreateGetUpdateAndDelete_UseExpectedStatusCodesAsync()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/jobapplications",
            CreateRequest("Contoso", "Developer"));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);
        var created = await createResponse.Content
            .ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync(
            $"/api/jobapplications/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/jobapplications/{created.Id}",
            new UpdateJobApplicationRequest(
                "Contoso",
                "Senior Developer",
                JobApplicationStatus.Interview,
                "Referral",
                DateTime.UtcNow.AddDays(-1),
                "Remote",
                120000,
                "https://example.com/jobs/1",
                "Updated"));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content
            .ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);
        Assert.Equal("Senior Developer", updated?.Role);
        Assert.Equal(JobApplicationStatus.Interview, updated?.Status);

        var deleteResponse = await _client.DeleteAsync(
            $"/api/jobapplications/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await _client.GetAsync(
                $"/api/jobapplications/{created.Id}")).StatusCode);
    }

    [Fact]
    public async Task MissingResources_ReturnNotFoundAsync()
    {
        var getResponse = await _client.GetAsync(
            "/api/jobapplications/999999");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        var problem = await getResponse.Content
            .ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Job application not found.", problem?.Title);
        Assert.Equal(
            "/api/jobapplications/999999",
            problem?.Instance);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await _client.PutAsJsonAsync(
                "/api/jobapplications/999999",
                UpdateRequest("Missing", "Role"))).StatusCode);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await _client.DeleteAsync("/api/jobapplications/999999"))
            .StatusCode);
    }

    [Fact]
    public async Task Delete_RepeatedRequest_ReturnsNotFoundAsync()
    {
        var created = await CreateAsync(
            "Repeat Delete",
            "Role",
            JobApplicationStatus.Applied,
            $"delete-{Guid.NewGuid():N}",
            -1);

        var first = await _client.DeleteAsync(
            $"/api/jobapplications/{created.Id}");
        var second = await _client.DeleteAsync(
            $"/api/jobapplications/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, second.StatusCode);
        var problem = await second.Content
            .ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Job application not found.", problem?.Title);
    }

    [Theory]
    [InlineData("/api/jobapplications?page=0")]
    [InlineData("/api/jobapplications?pageSize=0")]
    [InlineData("/api/jobapplications?pageSize=101")]
    [InlineData("/api/jobapplications?dateFrom=2026-08-01&dateTo=2026-07-30")]
    [InlineData("/api/jobapplications/not-an-integer")]
    [InlineData("/api/jobapplications/0")]
    public async Task InvalidInputs_ReturnValidationProblemAsync(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.NotEmpty(problem.Errors);
        Assert.Equal("Review the errors property for details.", problem.Detail);
        Assert.True(problem.Extensions.ContainsKey("traceId"));
    }

    [Theory]
    [InlineData(
        "/api/jobapplications?status=INVALID",
        "Status",
        "Applied")]
    [InlineData(
        "/api/jobapplications?sortBy=INVALID",
        "SortBy",
        "DateApplied")]
    [InlineData(
        "/api/jobapplications?sortDirection=INVALID",
        "SortDirection",
        "Ascending")]
    public async Task InvalidEnumQuery_ListsAcceptedNamesAsync(
        string url,
        string property,
        string acceptedValue)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        var error = Assert.Single(problem.Errors[property]);
        Assert.Contains(acceptedValue, error);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("null")]
    public async Task InvalidRequestBody_ReturnsValidationProblemAsync(
        string body)
    {
        using var content = new StringContent(
            body,
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync(
            "/api/jobapplications",
            content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.NotEmpty(problem.Errors);
        Assert.True(problem.Extensions.ContainsKey("traceId"));
    }

    [Fact]
    public async Task InvalidCreate_ReturnsValidationProblemDetailsAsync()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/jobapplications",
            CreateRequest(string.Empty, string.Empty) with
            {
                ExpectedSalary = -1,
                Link = "invalid",
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("CompanyName", problem.Errors.Keys);
        Assert.Contains("Role", problem.Errors.Keys);
    }

    [Fact]
    public async Task InvalidUpdate_ReturnsValidationProblemDetailsAsync()
    {
        var created = await CreateAsync(
            "Update Validation",
            "Role",
            JobApplicationStatus.Applied,
            $"update-{Guid.NewGuid():N}",
            -1);

        var response = await _client.PutAsJsonAsync(
            $"/api/jobapplications/{created.Id}",
            UpdateRequest("Valid Company", "   ") with
            {
                Status = (JobApplicationStatus)999,
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("Role", problem.Errors.Keys);
        Assert.Contains("Status", problem.Errors.Keys);
    }

    [Fact]
    public async Task List_SupportsPagingFilteringSearchingAndSortingAsync()
    {
        await CreateAsync("Zulu Corp", "Engineer",
            JobApplicationStatus.Applied, "LinkedIn", -3);
        await CreateAsync("Alpha Labs", "Architect",
            JobApplicationStatus.Interview, "Referral", -2);
        await CreateAsync("Alpha Labs", "Developer",
            JobApplicationStatus.Applied, "LinkedIn", -1);

        var dateFrom = Uri.EscapeDataString(
            DateTime.UtcNow.AddDays(-2).ToString("O"));
        var dateTo = Uri.EscapeDataString(DateTime.UtcNow.ToString("O"));
        var response = await _client.GetFromJsonAsync<
            PagedResult<JobApplicationDto>>(
            "/api/jobapplications?company=Alpha&status=Applied"
            + "&role=Developer&source=LinkedIn&search=Developer"
            + $"&dateFrom={dateFrom}&dateTo={dateTo}"
            + "&sortBy=CompanyName"
            + "&sortDirection=Ascending&page=1&pageSize=1",
            _jsonOptions);

        Assert.NotNull(response);
        Assert.Single(response.Items);
        Assert.Equal("Alpha Labs", response.Items[0].CompanyName);
        Assert.Equal("Developer", response.Items[0].Role);
        Assert.Equal(1, response.TotalItems);
        Assert.Equal(1, response.TotalPages);
        Assert.False(response.HasNextPage);
        Assert.False(response.HasPreviousPage);
    }

    [Fact]
    public async Task List_DefaultSortsByDateAppliedDescendingAsync()
    {
        await CreateAsync("Old Company", "Role",
            JobApplicationStatus.Screening, "Board", -10);
        await CreateAsync("New Company", "Role",
            JobApplicationStatus.Screening, "Board", -1);

        var response = await _client.GetFromJsonAsync<
            PagedResult<JobApplicationDto>>(
            "/api/jobapplications?status=Screening&pageSize=100",
            _jsonOptions);

        Assert.NotNull(response);
        Assert.True(response.Items.Count >= 2);
        Assert.Equal("New Company", response.Items[0].CompanyName);
    }

    [Fact]
    public async Task List_PaginatesFirstMiddleLastAndEmptyPagesAsync()
    {
        var source = $"paging-{Guid.NewGuid():N}";
        for (var index = 5; index >= 1; index--)
        {
            await CreateAsync(
                $"Paging {index}",
                "Role",
                JobApplicationStatus.Applied,
                source,
                -index);
        }

        var first = await GetPageAsync(source, page: 1, pageSize: 2);
        var middle = await GetPageAsync(source, page: 2, pageSize: 2);
        var last = await GetPageAsync(source, page: 3, pageSize: 2);
        var empty = await GetPageAsync(source, page: 4, pageSize: 2);

        Assert.Equal(5, first.TotalItems);
        Assert.Equal(3, first.TotalPages);
        Assert.False(first.HasPreviousPage);
        Assert.True(first.HasNextPage);
        Assert.Equal(2, middle.Items.Count);
        Assert.True(middle.HasPreviousPage);
        Assert.True(middle.HasNextPage);
        Assert.Single(last.Items);
        Assert.True(last.HasPreviousPage);
        Assert.False(last.HasNextPage);
        Assert.Empty(empty.Items);
        Assert.Empty(
            first.Items.Select(item => item.Id)
                .Intersect(middle.Items.Select(item => item.Id)));
    }

    [Fact]
    public async Task List_AcceptsMaximumPageSizeAsync()
    {
        var source = $"max-page-{Guid.NewGuid():N}";
        await CreateAsync(
            "Maximum Page",
            "Role",
            JobApplicationStatus.Applied,
            source,
            -1);

        var result = await GetPageAsync(source, page: 1, pageSize: 100);

        Assert.Equal(100, result.PageSize);
        Assert.Single(result.Items);
    }

    [Theory]
    [InlineData("CompanyName", "Alpha Company")]
    [InlineData("Role", "Zulu Company")]
    [InlineData("Status", "Applied Company")]
    [InlineData("DateApplied", "Older Company")]
    public async Task List_SortsEverySupportedFieldAscendingAsync(
        string sortBy,
        string expectedFirstCompany)
    {
        var source = $"sort-{sortBy}-{Guid.NewGuid():N}";
        await CreateAsync(
            "Zulu Company",
            "Alpha Role",
            JobApplicationStatus.Interview,
            source,
            -1);
        await CreateAsync(
            "Alpha Company",
            "Zulu Role",
            JobApplicationStatus.Interview,
            source,
            -2);
        await CreateAsync(
            "Applied Company",
            "Middle Role",
            JobApplicationStatus.Applied,
            source,
            -3);
        await CreateAsync(
            "Older Company",
            "Older Role",
            JobApplicationStatus.Offer,
            source,
            -10);

        var result = await _client.GetFromJsonAsync<
            PagedResult<JobApplicationDto>>(
            $"/api/jobapplications?source={source}&sortBy={sortBy}"
            + "&sortDirection=Ascending&pageSize=100",
            _jsonOptions);

        Assert.NotNull(result);
        Assert.Equal(expectedFirstCompany, result.Items[0].CompanyName);
    }

    [Fact]
    public async Task List_EnumQueryValuesAreCaseInsensitiveAsync()
    {
        var source = $"enum-case-{Guid.NewGuid():N}";
        await CreateAsync(
            "Case Insensitive",
            "Role",
            JobApplicationStatus.Applied,
            source,
            -1);

        var response = await _client.GetAsync(
            $"/api/jobapplications?source={source}&status=applied"
            + "&sortBy=role&sortDirection=ascending");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(JobApplicationStatus.Applied)]
    [InlineData(JobApplicationStatus.Screening)]
    [InlineData(JobApplicationStatus.Interview)]
    [InlineData(JobApplicationStatus.Offer)]
    [InlineData(JobApplicationStatus.Rejected)]
    public async Task List_FiltersEveryStatusAsync(
        JobApplicationStatus status)
    {
        var source = $"status-{status}-{Guid.NewGuid():N}";
        await CreateAsync("Matching", "Role", status, source, -1);
        await CreateAsync(
            "Different",
            "Role",
            status == JobApplicationStatus.Applied
                ? JobApplicationStatus.Rejected
                : JobApplicationStatus.Applied,
            source,
            -1);

        var result = await _client.GetFromJsonAsync<
            PagedResult<JobApplicationDto>>(
            $"/api/jobapplications?source={source}&status={status}",
            _jsonOptions);

        Assert.NotNull(result);
        var item = Assert.Single(result.Items);
        Assert.Equal(status, item.Status);
    }

    [Fact]
    public async Task List_DateToIncludesEntireBoundaryDateAsync()
    {
        var source = $"date-boundary-{Guid.NewGuid():N}";
        var date = new DateTime(2025, 7, 30, 15, 45, 0, DateTimeKind.Utc);
        var createResponse = await _client.PostAsJsonAsync(
            "/api/jobapplications",
            CreateRequest("Boundary Date", "Role") with
            {
                Source = source,
                DateApplied = date,
            });
        createResponse.EnsureSuccessStatusCode();

        var result = await _client.GetFromJsonAsync<
            PagedResult<JobApplicationDto>>(
            $"/api/jobapplications?source={source}&dateTo=2025-07-30",
            _jsonOptions);

        Assert.NotNull(result);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task StatusCounts_GroupsApplicationsByStatusAsync()
    {
        var before = await _client.GetFromJsonAsync<
            IReadOnlyList<StatusCountDto>>(
            "/api/jobapplications/status-counts",
            _jsonOptions);
        var beforeCount = before?
            .SingleOrDefault(item =>
                item.Status == JobApplicationStatus.Offer)
            ?.Count ?? 0;
        await CreateAsync("Offer One", "Role",
            JobApplicationStatus.Offer, "Referral", -2);
        await CreateAsync("Offer Two", "Role",
            JobApplicationStatus.Offer, "Referral", -1);

        var counts = await _client.GetFromJsonAsync<
            IReadOnlyList<StatusCountDto>>(
            "/api/jobapplications/status-counts",
            _jsonOptions);

        Assert.NotNull(counts);
        var offer = Assert.Single(
            counts,
            item => item.Status == JobApplicationStatus.Offer);
        Assert.Equal(beforeCount + 2, offer.Count);
    }

    [Fact]
    public async Task SwaggerUiAndOpenApiDocument_ExposeEveryEndpointAsync()
    {
        var swaggerUi = await _client.GetAsync("/swagger/index.html");
        Assert.Equal(HttpStatusCode.OK, swaggerUi.StatusCode);

        using var document = JsonDocument.Parse(
            await _client.GetStringAsync("/openapi/v1.json"));
        var paths = document.RootElement.GetProperty("paths");

        AssertOpenApiOperation(
            paths,
            "/api/jobapplications",
            "get",
            "Gets a filtered, sorted, and paged list of job applications.",
            "200",
            "400",
            "401",
            "403",
            "500",
            "503");
        AssertOpenApiOperation(
            paths,
            "/api/jobapplications",
            "post",
            "Creates a job application.",
            "201",
            "400",
            "401",
            "403",
            "500",
            "503");
        AssertOpenApiOperation(
            paths,
            "/api/jobapplications/{id}",
            "get",
            "Gets one job application by its identifier.",
            "200",
            "400",
            "401",
            "403",
            "404",
            "500",
            "503");
        AssertOpenApiOperation(
            paths,
            "/api/jobapplications/{id}",
            "put",
            "Updates an existing job application.",
            "200",
            "400",
            "401",
            "403",
            "404",
            "500",
            "503");
        AssertOpenApiOperation(
            paths,
            "/api/jobapplications/{id}",
            "delete",
            "Deletes a job application.",
            "204",
            "400",
            "401",
            "403",
            "404",
            "500",
            "503");
        AssertOpenApiOperation(
            paths,
            "/api/jobapplications/status-counts",
            "get",
            "Gets application counts grouped by status.",
            "200",
            "401",
            "403",
            "500",
            "503");

        var listParameters = paths
            .GetProperty("/api/jobapplications")
            .GetProperty("get")
            .GetProperty("parameters")
            .EnumerateArray()
            .Select(parameter => parameter.GetProperty("name").GetString())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var expectedParameters = new[]
        {
            "page",
            "pageSize",
            "sortBy",
            "sortDirection",
            "status",
            "company",
            "role",
            "source",
            "search",
            "dateFrom",
            "dateTo",
        };

        Assert.All(
            expectedParameters,
            parameter => Assert.Contains(parameter, listParameters));

        var components = document.RootElement.GetProperty("components");
        var schemas = components.GetProperty("schemas");
        var statusSchema = schemas.GetProperty("JobApplicationStatus");
        Assert.Equal("string", statusSchema.GetProperty("type").GetString());
        Assert.Contains(
            statusSchema.GetProperty("enum").EnumerateArray(),
            value => value.GetString() == "Applied");
        AssertStringEnum(
            schemas,
            "JobApplicationSortBy",
            "DateApplied",
            "CompanyName",
            "Role",
            "Status");
        AssertStringEnum(
            schemas,
            "SortDirection",
            "Ascending",
            "Descending");

        var listOperation = paths
            .GetProperty("/api/jobapplications")
            .GetProperty("get");
        AssertParameter(
            listOperation,
            "page",
            "Page number starting at 1. Default: 1.");
        AssertParameter(
            listOperation,
            "pageSize",
            "Number of records returned per page, from 1 to 100. "
            + "Default: 20.");
        AssertParameter(
            listOperation,
            "status",
            "Filters applications by status.");
        AssertParameter(
            listOperation,
            "sortBy",
            "Field used for sorting.");
        AssertParameter(
            listOperation,
            "sortDirection",
            "Sort order. Accepted values: Ascending or Descending. "
            + "Default: Descending.");
        AssertDateParameter(listOperation, "dateFrom");
        AssertDateParameter(listOperation, "dateTo");

        var createSchema = schemas.GetProperty(
            "CreateJobApplicationRequest");
        var createExample = createSchema.GetProperty("example");
        Assert.Equal(
            "Contoso Ltd.",
            createExample.GetProperty("companyName").GetString());
        Assert.Equal(
            "Applied",
            createExample.GetProperty("status").GetString());
        Assert.Equal(
            "2026-07-30",
            createExample.GetProperty("dateApplied").GetString());
        Assert.Equal(
            "date",
            createSchema
                .GetProperty("properties")
                .GetProperty("dateApplied")
                .GetProperty("format")
                .GetString());

        var updateSchema = schemas.GetProperty(
            "UpdateJobApplicationRequest");
        var updateExample = updateSchema.GetProperty("example");
        Assert.Equal(
            "Interview",
            updateExample.GetProperty("status").GetString());

        var auth0Scheme = components
            .GetProperty("securitySchemes")
            .GetProperty("Auth0");
        Assert.Equal(
            "oauth2",
            auth0Scheme.GetProperty("type").GetString());
        var authorizationCode = auth0Scheme
            .GetProperty("flows")
            .GetProperty("authorizationCode");
        Assert.Equal(
            "https://jobtracker-test.auth0.com/authorize",
            authorizationCode
                .GetProperty("authorizationUrl")
                .GetString());
        Assert.Equal(
            "https://jobtracker-test.auth0.com/oauth/token",
            authorizationCode.GetProperty("tokenUrl").GetString());
        Assert.True(
            authorizationCode
                .GetProperty("scopes")
                .TryGetProperty("openid", out _));
        Assert.True(listOperation.GetProperty("security").GetArrayLength() > 0);
    }

    private async Task<JobApplicationDto> CreateAsync(
        string company,
        string role,
        JobApplicationStatus status,
        string source,
        int daysAgo)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/jobapplications",
            CreateRequest(company, role) with
            {
                Status = status,
                Source = source,
                DateApplied = DateTime.UtcNow.AddDays(daysAgo),
            });
        response.EnsureSuccessStatusCode();
        var created = await response.Content
            .ReadFromJsonAsync<JobApplicationDto>(_jsonOptions);
        return Assert.IsType<JobApplicationDto>(created);
    }

    private async Task<PagedResult<JobApplicationDto>> GetPageAsync(
        string source,
        int page,
        int pageSize)
    {
        var result = await _client.GetFromJsonAsync<
            PagedResult<JobApplicationDto>>(
            $"/api/jobapplications?source={source}"
            + "&sortBy=DateApplied&sortDirection=Ascending"
            + $"&page={page}&pageSize={pageSize}",
            _jsonOptions);
        return Assert.IsType<PagedResult<JobApplicationDto>>(result);
    }

    private static CreateJobApplicationRequest CreateRequest(
        string company,
        string role) =>
        new(
            company,
            role,
            JobApplicationStatus.Applied,
            "LinkedIn",
            DateTime.UtcNow.AddDays(-1),
            "Remote",
            100000,
            "https://example.com/job",
            "Notes");

    private static UpdateJobApplicationRequest UpdateRequest(
        string company,
        string role) =>
        new(
            company,
            role,
            JobApplicationStatus.Applied,
            null,
            DateTime.UtcNow.AddDays(-1),
            null,
            null,
            null,
            null);

    private static void AssertOpenApiOperation(
        JsonElement paths,
        string path,
        string method,
        string summary,
        params string[] responseCodes)
    {
        var operation = paths.GetProperty(path).GetProperty(method);
        Assert.Equal(summary, operation.GetProperty("summary").GetString());

        var responses = operation.GetProperty("responses");
        Assert.All(
            responseCodes,
            responseCode => Assert.True(
                responses.TryGetProperty(responseCode, out _),
                $"Response {responseCode} is missing from {method} {path}."));
    }

    private static void AssertStringEnum(
        JsonElement schemas,
        string schemaName,
        params string[] values)
    {
        var schema = schemas.GetProperty(schemaName);
        Assert.Equal("string", schema.GetProperty("type").GetString());
        var actual = schema
            .GetProperty("enum")
            .EnumerateArray()
            .Select(value => value.GetString())
            .ToHashSet(StringComparer.Ordinal);
        Assert.All(values, value => Assert.Contains(value, actual));
    }

    private static void AssertParameter(
        JsonElement operation,
        string parameterName,
        string description)
    {
        var parameter = operation
            .GetProperty("parameters")
            .EnumerateArray()
            .Single(candidate => string.Equals(
                candidate.GetProperty("name").GetString(),
                parameterName,
                StringComparison.OrdinalIgnoreCase));
        Assert.Equal(
            description,
            parameter.GetProperty("description").GetString());
    }

    private static void AssertDateParameter(
        JsonElement operation,
        string parameterName)
    {
        var parameter = operation
            .GetProperty("parameters")
            .EnumerateArray()
            .Single(candidate => string.Equals(
                candidate.GetProperty("name").GetString(),
                parameterName,
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            "Format: yyyy-MM-dd.",
            parameter.GetProperty("description").GetString());
        Assert.Equal(
            "date",
            parameter.GetProperty("schema").GetProperty("format").GetString());
    }
}
