using Microsoft.Extensions.Options;

namespace JobTracker.Api.Authentication;

public static class Auth0SwaggerApplicationExtensions
{
    public static IApplicationBuilder UseAuth0Swagger(
        this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var auth0 = app.Services
            .GetRequiredService<IOptions<Auth0Options>>()
            .Value;

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(
                "/openapi/v1.json",
                "JobTracker API v1");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "JobTracker API";
            options.OAuthClientId(auth0.ClientId);
            options.OAuthScopes(
                auth0.Scopes
                    .Distinct(StringComparer.Ordinal)
                    .ToArray());
            options.OAuthUsePkce();
            options.OAuthAdditionalQueryStringParams(
                new Dictionary<string, string>
                {
                    ["audience"] = auth0.Audience,
                });
        });

        return app;
    }
}
