using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace JobTracker.Api.Configuration;

public sealed class JobTrackerOpenApiOperationTransformer
    : IOpenApiOperationTransformer
{
    private static readonly string[] _dateParameterNames =
    [
        "dateFrom",
        "dateTo",
    ];

    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        foreach (var parameter in operation.Parameters ?? [])
        {
            if (!_dateParameterNames.Contains(
                    parameter.Name,
                    StringComparer.OrdinalIgnoreCase)
                || parameter.Schema is not OpenApiSchema schema)
            {
                continue;
            }

            schema.Format = "date";
            schema.Example = JsonValue.Create("2026-07-30");
        }

        return Task.CompletedTask;
    }
}
