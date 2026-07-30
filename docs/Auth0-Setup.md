# Auth0 authentication setup

The JobTracker API validates Auth0 JWT access tokens. Swagger UI is a public
OAuth client and uses Authorization Code Flow with PKCE. No client secret is
used or stored by this repository.

## Authentication flow

```text
Swagger UI
    |
    v
Auth0 Universal Login
    |
    +--> Google
    |
    +--> GitHub
    |
    v
Auth0 JWT access token
    |
    v
JobTracker API
```

Auth0 authenticates the user through Universal Login. Swagger exchanges the
authorization code using PKCE and stores the resulting access token in the
browser session. Swagger then sends the token as:

```http
Authorization: Bearer ACCESS_TOKEN
```

The API validates the token issuer, audience, signature, and lifetime.

## 1. Create the Auth0 API

In **Auth0 Dashboard > Applications > APIs**, create an API:

- Name: `JobTracker API`
- Identifier: `https://jobtracker-api` or another stable URI
- Signing Algorithm: `RS256`

The Identifier is the API audience. It does not need to be a reachable URL.
Copy it into `Auth0:Audience`.

No API permissions are required for the current authentication-only business
requirement.

## 2. Create the Swagger OAuth application

In **Auth0 Dashboard > Applications > Applications**, create an application:

- Name: `JobTracker Swagger`
- Application Type: **Single Page Application**
- Token Endpoint Authentication Method: **None**
- Grant Type: **Authorization Code**

Copy its Client ID into `Auth0:ClientId`. Do not add a client secret to the
API configuration. Swagger is a browser-based public client and uses PKCE.

For the checked-in development launch profiles, configure:

### Allowed Callback URLs

```text
https://localhost:7104/swagger/oauth2-redirect.html,
http://localhost:5032/swagger/oauth2-redirect.html
```

The scheme, host, port, path, and trailing slash behavior must exactly match
the URL used by Swagger.

### Allowed Logout URLs

Swagger's **Logout** button clears its local authorization state and does not
currently invoke the Auth0 logout endpoint. If interactive Auth0 logout is
added later, use:

```text
https://localhost:7104/swagger,
http://localhost:5032/swagger
```

### Allowed Web Origins

```text
https://localhost:7104,
http://localhost:5032
```

Origins contain only scheme, host, and port. Do not include `/swagger`.

### Allowed Origins (CORS)

```text
https://localhost:7104,
http://localhost:5032
```

Use exact production URLs rather than localhost or wildcards outside local
development.

## 3. Configure the API

Replace the placeholders under `Auth0` using user secrets, environment
variables, or a secret manager:

```json
{
  "Auth0": {
    "Authority": "https://YOUR_TENANT.REGION.auth0.com/",
    "Audience": "https://jobtracker-api",
    "ClientId": "YOUR_SWAGGER_SPA_CLIENT_ID",
    "Scopes": [
      "openid",
      "profile",
      "email"
    ]
  }
}
```

`Authority` must use HTTPS and end with `/`.

Example environment variables:

```powershell
$env:Auth0__Authority = "https://YOUR_TENANT.REGION.auth0.com/"
$env:Auth0__Audience = "https://jobtracker-api"
$env:Auth0__ClientId = "YOUR_SWAGGER_SPA_CLIENT_ID"
```

The Client ID is public OAuth metadata, but keeping environment-specific
configuration outside source control avoids accidental tenant coupling.
Google, GitHub, and Auth0 client secrets must never be stored in this
repository.

## 4. Configure Google login

### Google Cloud Console

1. Open **Google Auth Platform** for the appropriate Google Cloud project.
2. Configure Branding and the OAuth consent screen.
3. Choose an External audience when users may be outside your organization.
4. While the consent screen is in testing, add the required Google accounts
   as test users.
5. Create an OAuth client with application type **Web application**.
6. Set the authorized JavaScript origin to the Auth0 origin:

   ```text
   https://YOUR_AUTH0_DOMAIN
   ```

7. Set the authorized redirect URI to:

   ```text
   https://YOUR_AUTH0_DOMAIN/login/callback
   ```

8. Copy the Google Client ID and Client Secret.

The Google callback points to Auth0, not directly to Swagger.

### Auth0

1. Open **Authentication > Social > Create Connection**.
2. Select **Google / Gmail**.
3. Enter the Google Client ID and Client Secret.
4. Enable the basic profile/email permissions needed by the application.
5. In the connection's **Applications** tab, enable the connection for
   `JobTracker Swagger`.
6. Use **Try Connection** to verify the Google flow.

## 5. Configure GitHub login

### GitHub

1. Open **Settings > Developer settings > OAuth Apps**.
2. Select **New OAuth App**.
3. Use a recognizable application name such as `JobTracker via Auth0`.
4. Set the Homepage URL to the application or Swagger URL.
5. Set the Authorization callback URL to:

   ```text
   https://YOUR_AUTH0_DOMAIN/login/callback
   ```

6. Register the application.
7. Copy the Client ID.
8. Generate and securely copy the Client Secret.

The GitHub callback points to Auth0, not directly to Swagger.

### Auth0

1. Open **Authentication > Social > Create Connection**.
2. Select **GitHub**.
3. Enter the GitHub Client ID and Client Secret.
4. Use the connection for login.
5. In the connection's **Applications** tab, enable it for
   `JobTracker Swagger`.
6. Use **Try Connection** to verify the GitHub flow.

## 6. Manual Swagger verification

1. Trust the local HTTPS certificate if required:

   ```powershell
   dotnet dev-certs https --trust
   ```

2. Start the API.
3. Open `https://localhost:7104/swagger`.
4. Select **Authorize**.
5. Select the `openid`, `profile`, and `email` scopes.
6. Start authorization.
7. Confirm Auth0 Universal Login shows the enabled Google and GitHub
   connections.
8. Sign in with either provider.
9. Confirm the browser returns to:

   ```text
   https://localhost:7104/swagger/oauth2-redirect.html
   ```

10. Execute `GET /api/jobapplications`.
11. In the browser network tools, confirm the request includes an
    `Authorization: Bearer ...` header.
12. Select **Logout** in Swagger and call the endpoint again. It must return
    `401 Unauthorized`.

## Authentication without role authorization

Every JobApplications endpoint requires an authenticated principal, but the
application intentionally defines no roles, policies, permissions, or RBAC.
This matches the current single-user requirement.

`UseAuthorization()` is still required. ASP.NET Core's authorization
middleware evaluates the `[Authorize]` metadata and enforces the default rule
that a user must be authenticated. Authentication establishes identity;
authorization decides whether that identity may reach the endpoint. Here the
authorization decision is intentionally limited to "authenticated or not."

## Troubleshooting

### `401 Unauthorized`

- Confirm the access token `iss` exactly matches `Auth0:Authority`.
- Confirm the token `aud` contains `Auth0:Audience`.
- Confirm Swagger sends an access token, not an ID token.
- Confirm the Auth0 API uses RS256.
- Confirm the token has not expired.

### `redirect_uri_mismatch`

Copy the redirect URI shown in the Auth0 error and add that exact value to
the Swagger Auth0 application's Allowed Callback URLs.

### Google or GitHub is missing from Universal Login

Confirm the social connection is enabled for the `JobTracker Swagger`
application in the connection's **Applications** tab.

### Auth0 starts but returns an opaque access token

Confirm Swagger sends the configured API audience. The implementation adds
`Auth0:Audience` to the authorization request automatically.
