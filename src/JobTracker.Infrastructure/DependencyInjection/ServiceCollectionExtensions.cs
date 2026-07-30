using JobTracker.Application.JobApplications.Persistence;
using JobTracker.Infrastructure.Data;
using JobTracker.Infrastructure.JobApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for configuring infrastructure services in the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDbContext<JobTrackerDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("JobTracker"),
                sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
        });
        services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

        return services;
    }
}
