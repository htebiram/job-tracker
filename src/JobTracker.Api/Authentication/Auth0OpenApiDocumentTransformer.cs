using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace JobTracker.Api.Authentication;

public sealed class Auth0OpenApiDocumentTransformer(
    IOptions<Auth0Options> options) : IOpenApiDocumentTransformer
{
    private const string SECURITY_SCHEME_NAME = "Auth0";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(context);

        var auth0 = options.Value;
        var requestedScopes = auth0.Scopes
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        var scopes = requestedScopes.ToDictionary(
            scope => scope,
            scope => GetScopeDescription(scope),
            StringComparer.Ordinal);
        var securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Description = "Auth0 Authorization Code flow with PKCE.",
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(
                        new Uri(auth0.Authority),
                        "authorize"),
                    TokenUrl = new Uri(
                        new Uri(auth0.Authority),
                        "oauth/token"),
                    Scopes = scopes,
                },
            },
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes =
            new Dictionary<string, IOpenApiSecurityScheme>
            {
                [SECURITY_SCHEME_NAME] = securityScheme,
            };

        if (document.Paths is null)
        {
            return Task.CompletedTask;
        }

        foreach (var path in document.Paths.Values)
        {
            if (path.Operations is null)
            {
                continue;
            }

            foreach (var operation in path.Operations.Values)
            {
                operation.Security ??= [];
                operation.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(
                            SECURITY_SCHEME_NAME,
                        document)] = requestedScopes.ToList(),
                    });
            }
        }

        return Task.CompletedTask;
    }

    private static string GetScopeDescription(string scope) =>
        scope switch
        {
            "openid" => "Authenticate using OpenID Connect.",
            "profile" => "Read the authenticated user's basic profile.",
            "email" => "Read the authenticated user's email address.",
            _ => $"Request the '{scope}' API scope.",
        };
}
