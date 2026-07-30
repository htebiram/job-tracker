using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace JobTracker.Tests.Integration;

public sealed class JwtAuthenticationTests
    : IClassFixture<JobTrackerApiFactory>
{
    private const string ISSUER = "https://jobtracker-test.auth0.com/";
    private const string AUDIENCE = "https://jobtracker.test/api";
    private static readonly SymmetricSecurityKey _signingKey = new(
        Encoding.UTF8.GetBytes(
            "jobtracker-integration-test-signing-key-2026"));

    private readonly JobTrackerApiFactory _factory;

    public JwtAuthenticationTests(JobTrackerApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ValidBearerToken_AuthenticatesRequestAsync()
    {
        using var client = CreateBearerClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                CreateToken(ISSUER, AUDIENCE, _signingKey, DateTime.UtcNow
                    .AddMinutes(5)));

        var response = await client.GetAsync("/api/jobapplications");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TokenFailure.Expired)]
    [InlineData(TokenFailure.WrongIssuer)]
    [InlineData(TokenFailure.WrongAudience)]
    [InlineData(TokenFailure.WrongSignature)]
    [InlineData(TokenFailure.Malformed)]
    public async Task InvalidBearerToken_ReturnsSafeUnauthorizedProblemAsync(
        TokenFailure failure)
    {
        using var client = CreateBearerClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                CreateInvalidToken(failure));

        var response = await client.GetAsync("/api/jobapplications");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Authentication required", problem?.Title);
        Assert.Equal(
            "A valid access token is required to access this resource.",
            problem?.Detail);
        Assert.True(problem?.Extensions.ContainsKey("traceId"));
    }

    [Fact]
    public async Task WrongAuthenticationScheme_ReturnsUnauthorizedAsync()
    {
        using var client = CreateBearerClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", "dXNlcjpwYXNzd29yZA==");

        var response = await client.GetAsync("/api/jobapplications");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private HttpClient CreateBearerClient()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    options.DefaultScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                });
                services.PostConfigure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Authority = null;
                        options.ConfigurationManager = null;
                        options.TokenValidationParameters.ValidIssuer = ISSUER;
                        options.TokenValidationParameters.ValidAudience =
                            AUDIENCE;
                        options.TokenValidationParameters.IssuerSigningKey =
                            _signingKey;
                        options.TokenValidationParameters.ClockSkew =
                            TimeSpan.Zero;
                    });
            }));

        return factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });
    }

    private static string CreateInvalidToken(TokenFailure failure) =>
        failure switch
        {
            TokenFailure.Expired => CreateToken(
                ISSUER,
                AUDIENCE,
                _signingKey,
                DateTime.UtcNow.AddMinutes(-1)),
            TokenFailure.WrongIssuer => CreateToken(
                "https://wrong-issuer.example/",
                AUDIENCE,
                _signingKey,
                DateTime.UtcNow.AddMinutes(5)),
            TokenFailure.WrongAudience => CreateToken(
                ISSUER,
                "https://wrong-audience.example/api",
                _signingKey,
                DateTime.UtcNow.AddMinutes(5)),
            TokenFailure.WrongSignature => CreateToken(
                ISSUER,
                AUDIENCE,
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        "different-integration-signing-key-2026")),
                DateTime.UtcNow.AddMinutes(5)),
            _ => "not-a-jwt",
        };

    private static string CreateToken(
        string issuer,
        string audience,
        SecurityKey signingKey,
        DateTime expires)
    {
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer,
            audience,
            [new Claim("sub", "auth0|jwt-integration-user")],
            notBefore: expires <= now ? now.AddMinutes(-10) : now.AddMinutes(-1),
            expires,
            new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public enum TokenFailure
    {
        Expired,
        WrongIssuer,
        WrongAudience,
        WrongSignature,
        Malformed,
    }
}
