namespace JobTracker.Api.Authentication;

public sealed class Auth0Options
{
    public const string SECTION_NAME = "Auth0";

    public string Authority { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string ClientId { get; init; } = string.Empty;

    public string[] Scopes { get; init; } =
    [
        "openid",
        "profile",
        "email",
    ];
}
