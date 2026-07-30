using System.Text.Json.Nodes;
using JobTracker.Application.JobApplications.Models;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace JobTracker.Api.Configuration;

public sealed class JobTrackerOpenApiSchemaTransformer
    : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        if (context.JsonTypeInfo.Type.IsEnum)
        {
            schema.Type = JsonSchemaType.String;
            schema.Enum = Enum
                .GetNames(context.JsonTypeInfo.Type)
                .Select(name => (JsonNode)JsonValue.Create(name)!)
                .ToList();
        }

        schema.Example = context.JsonTypeInfo.Type switch
        {
            var type when type == typeof(CreateJobApplicationRequest) =>
                JsonNode.Parse(CREATE_REQUEST_EXAMPLE),
            var type when type == typeof(UpdateJobApplicationRequest) =>
                JsonNode.Parse(UPDATE_REQUEST_EXAMPLE),
            _ => schema.Example,
        };

        if (context.JsonTypeInfo.Type
            is var requestType
            && (requestType == typeof(CreateJobApplicationRequest)
                || requestType == typeof(UpdateJobApplicationRequest))
            && schema.Properties?.TryGetValue(
                "dateApplied",
                out var dateApplied) == true
            && dateApplied is OpenApiSchema dateAppliedSchema)
        {
            dateAppliedSchema.Format = "date";
            dateAppliedSchema.Description =
                "Application date. Format: yyyy-MM-dd. "
                + "Example: 2026-07-30.";
        }

        return Task.CompletedTask;
    }

    private const string CREATE_REQUEST_EXAMPLE =
        """
        {
          "companyName": "Contoso Ltd.",
          "role": "Senior .NET Developer",
          "status": "Applied",
          "source": "LinkedIn",
          "dateApplied": "2026-07-30",
          "location": "Makati City / Hybrid",
          "expectedSalary": 120000,
          "link": "https://example.com/jobs/senior-dotnet-developer",
          "notes": "Submitted through the company careers page."
        }
        """;

    private const string UPDATE_REQUEST_EXAMPLE =
        """
        {
          "companyName": "Contoso Ltd.",
          "role": "Senior .NET Developer",
          "status": "Interview",
          "source": "LinkedIn",
          "dateApplied": "2026-07-30",
          "location": "Makati City / Hybrid",
          "expectedSalary": 130000,
          "link": "https://example.com/jobs/senior-dotnet-developer",
          "notes": "Technical interview scheduled."
        }
        """;
}
