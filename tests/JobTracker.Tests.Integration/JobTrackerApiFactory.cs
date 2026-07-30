using JobTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;

namespace JobTracker.Tests.Integration;

public sealed class JobTrackerApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"JobTracker-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((context, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Auth0:Authority"] =
                        "https://jobtracker-test.auth0.com/",
                    ["Auth0:Audience"] = "https://jobtracker.test/api",
                    ["Auth0:ClientId"] = "swagger-test-client",
                    ["Auth0:Scopes:0"] = "openid",
                    ["Auth0:Scopes:1"] = "profile",
                    ["Auth0:Scopes:2"] = "email",
                });
        });
        builder.ConfigureServices(services =>
        {
            services.AddDataProtection()
                .UseEphemeralDataProtectionProvider();
            services.RemoveAll<DbContextOptions<JobTrackerDbContext>>();
            services.RemoveAll<JobTrackerDbContext>();
            services.AddDbContext<JobTrackerDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        TestAuthenticationHandler.SCHEME;
                    options.DefaultChallengeScheme =
                        TestAuthenticationHandler.SCHEME;
                })
                .AddScheme<AuthenticationSchemeOptions,
                    TestAuthenticationHandler>(
                    TestAuthenticationHandler.SCHEME,
                    options => { });
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme =
                    TestAuthenticationHandler.SCHEME;
                options.DefaultChallengeScheme =
                    TestAuthenticationHandler.SCHEME;
                options.DefaultScheme =
                    TestAuthenticationHandler.SCHEME;
            });
        });
    }
}
