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
                ClientName = "Swagger UI (All GameNest Services)",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                AllowAccessTokensViaBrowser = true,
                AllowOfflineAccess = true,
                AccessTokenLifetime = 3600,

                RedirectUris =
                {
                    // Order Service
                    "https://localhost:7045/swagger/oauth2-redirect.html",
                    "https://localhost:7122/swagger/oauth2-redirect.html",

                    // Catalog Service
                    "https://localhost:7048/swagger/oauth2-redirect.html",
                    "https://localhost:7046/swagger/oauth2-redirect.html",

                    // Review Service
                    "https://localhost:7047/swagger/oauth2-redirect.html",
                    "https://localhost:7260/swagger/oauth2-redirect.html",

                    // Cart Service
                    "https://localhost:7050/swagger/oauth2-redirect.html",
                    "https://localhost:7162/swagger/oauth2-redirect.html",

                    // Aggregator Service
                    "https://localhost:7049/swagger/oauth2-redirect.html",
                    "https://localhost:7111/swagger/oauth2-redirect.html",

                    // Gateway
                    "https://localhost:5000/swagger/oauth2-redirect.html"
                },

                AllowedCorsOrigins =
                {
                    "https://localhost:7045",
                    "https://localhost:7122",
                    "https://localhost:7048",
                    "https://localhost:7046",
                    "https://localhost:7047",
                    "https://localhost:7260",
                    "https://localhost:7050",
                    "https://localhost:7162",
                    "https://localhost:7049",
                    "https://localhost:7111",
                    "https://localhost:5000"
                },

                AllowedScopes = { "openid", "profile", "email", "gamenest_api" }
            },

            new Client
            {
                ClientId = "postman",
                ClientName = "Postman Testing Client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("postman-secret".Sha256()) },
                AllowAccessTokensViaBrowser = true,
                AccessTokenLifetime = 3600,
                AllowedScopes = { "openid", "profile", "email", "gamenest_api" }
            }
        };
}