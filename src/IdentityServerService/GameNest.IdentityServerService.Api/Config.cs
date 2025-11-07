using Duende.IdentityServer.Models;

namespace GameNest.IdentityServerService.Api;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope("gamenest_api", "Full access to all GameNest services")
            {
                UserClaims = { "role", "name", "email" }
            }
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new[]
        {
            new ApiResource("gamenest_api", "GameNest API")
            {
                Scopes = { "gamenest_api" },
                UserClaims = { "role", "name", "email" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            new Client
            {
                ClientId = "swagger",
                ClientName = "Swagger UI (All Services)",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                RedirectUris =
                {
                    "https://localhost:7045/swagger/oauth2-redirect.html",
                    "https://localhost:7048/swagger/oauth2-redirect.html",
                    "https://localhost:7047/swagger/oauth2-redirect.html",
                    "https://localhost:7049/swagger/oauth2-redirect.html",
                    "https://localhost:7050/swagger/oauth2-redirect.html",
                    "https://localhost:7051/swagger/oauth2-redirect.html",
                    "https://localhost:7052/swagger/oauth2-redirect.html"
                },
                AllowedCorsOrigins =
                {
                    "https://localhost:7045",
                    "https://localhost:7048",
                    "https://localhost:7047",
                    "https://localhost:7049",
                    "https://localhost:7050",
                    "https://localhost:7051",
                    "https://localhost:7052"
                },
                AllowedScopes = { "openid", "profile", "gamenest_api" },
                AllowOfflineAccess = true,
                AllowAccessTokensViaBrowser = true,
                AccessTokenLifetime = 3600
            },
            new Client
            {
                ClientId = "postman",
                ClientName = "Postman Testing Client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowAccessTokensViaBrowser = true,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "openid", "profile", "email", "gamenest_api" },
                AccessTokenLifetime = 3600
            }
        };
}