using FluentValidation;
using JobTracker.Application.JobApplications.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Application.DependencyInjection;

/// <summary>
/// Extension methods for configuring application services in the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddValidatorsFromAssemblyContaining(
            typeof(ServiceCollectionExtensions));
        services.AddScoped<IJobApplicationService, JobApplicationService>();

        return services;
    }
}
