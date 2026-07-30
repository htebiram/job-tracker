using JobTracker.Api.Authentication;
using JobTracker.Api.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace JobTracker.Api.Configuration;

/// <summary>
/// Extension methods for configuring API services in the service collection.
/// </summary>
public static class ApiServiceCollectionExtensions
{
    /// <summary>
    /// Adds API services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ValidationFilter>();
        services.AddControllers(options =>
            options.Filters.AddService<ValidationFilter>())
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter()));
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                return new BadRequestObjectResult(
                    ApiProblemDetailsFactory.CreateValidation(
                        context.HttpContext,
                        context.ModelState));
            };
        });
        services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer<JobTrackerOpenApiSchemaTransformer>();
            options.AddOperationTransformer<
                JobTrackerOpenApiOperationTransformer>();
            options.AddDocumentTransformer<Auth0OpenApiDocumentTransformer>();
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] =
                    context.HttpContext.TraceIdentifier;

                context.ProblemDetails.Extensions["timestamp"] =
                    DateTimeOffset.UtcNow;
            };
        });

        services.AddHealthChecks();

        return services;
    }
}
