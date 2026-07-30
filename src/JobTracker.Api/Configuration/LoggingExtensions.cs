using Serilog;

namespace JobTracker.Api.Configuration;

/// <summary>
/// Provides extensions for configuring application logging.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Configures Serilog as the application's logging provider.
    /// </summary>
    public static IHostApplicationBuilder AddLoggingServices(
        this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSerilog((services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });

        return builder;
    }
}
