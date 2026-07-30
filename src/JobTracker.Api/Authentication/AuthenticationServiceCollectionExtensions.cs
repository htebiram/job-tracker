using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using JobTracker.Api.Middleware;

namespace JobTracker.Api.Authentication;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0Authentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<Auth0Options>()
            .Bind(configuration.GetSection(Auth0Options.SECTION_NAME))
            .Validate(
                options => IsValid(options),
                "Auth0 Authority, Audience, ClientId, and Scopes must be "
                + "configured with non-placeholder values.")
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme)
            .Configure<Microsoft.Extensions.Options.IOptions<Auth0Options>>((
                options,
                auth0Options) =>
            {
                var auth0 = auth0Options.Value;
                options.Authority = auth0.Authority;
                options.Audience = auth0.Audience;
                options.RequireHttpsMetadata = true;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = auth0.Authority,
                    ValidateAudience = true,
                    ValidAudience = auth0.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = "sub",
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        var problem = ApiProblemDetailsFactory.Create(
                            context.HttpContext,
                            StatusCodes.Status401Unauthorized,
                            "Authentication required",
                            "A valid access token is required to access "
                            + "this resource.");
                        await ApiProblemDetailsFactory.WriteAsync(
                            context.HttpContext,
                            problem);
                    },
                    OnForbidden = async context =>
                    {
                        var problem = ApiProblemDetailsFactory.Create(
                            context.HttpContext,
                            StatusCodes.Status403Forbidden,
                            "Forbidden",
                            "You do not have permission to access this "
                            + "resource.");
                        await ApiProblemDetailsFactory.WriteAsync(
                            context.HttpContext,
                            problem);
                    },
                };
            });

        services.AddAuthorization();

        return services;
    }

    private static bool IsValid(Auth0Options options) =>
        Uri.TryCreate(
            options.Authority,
            UriKind.Absolute,
            out var authority)
        && authority.Scheme == Uri.UriSchemeHttps
        && options.Authority.EndsWith(
            "/",
            StringComparison.Ordinal)
        && !options.Authority.Contains(
            "YOUR_",
            StringComparison.OrdinalIgnoreCase)
        && !string.IsNullOrWhiteSpace(options.Audience)
        && !options.Audience.Contains(
            "YOUR_",
            StringComparison.OrdinalIgnoreCase)
        && !string.IsNullOrWhiteSpace(options.ClientId)
        && !options.ClientId.Contains(
            "YOUR_",
            StringComparison.OrdinalIgnoreCase)
        && options.Scopes.Length > 0
        && options.Scopes.All(scope => !string.IsNullOrWhiteSpace(scope));
}
