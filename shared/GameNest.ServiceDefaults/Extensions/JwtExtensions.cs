using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace GameNest.ServiceDefaults.Extensions
{
    public static class JwtExtensions
    {
        /// <summary>
        /// Adds Keycloak JWT Bearer authentication to the service collection.
        /// Reads configuration from "Keycloak:Url" and "Keycloak:Realm" settings.
        /// </summary>
        public static IServiceCollection AddKeycloakJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var keycloakUrl = configuration["Keycloak:Url"]
                           ?? configuration["services:keycloak:http:0"]
                           ?? "http://localhost:8080";

            var realm = configuration["Keycloak:Realm"] ?? "GameNest";
            var audience = configuration["Keycloak:Audience"] ?? "gamenest_api";

            var authority = $"{keycloakUrl}/realms/{realm}";

            if (string.IsNullOrWhiteSpace(keycloakUrl) || string.IsNullOrWhiteSpace(realm))
            {
                throw new InvalidOperationException(
                    "Keycloak configuration is missing. " +
                    "Please set 'Keycloak:Url' and 'Keycloak:Realm' in appsettings.json " +
                    "or ensure Aspire provides the keycloak connection.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.Audience = audience;
                    options.RequireHttpsMetadata = false; // For local dev with HTTP

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidAudiences = new[]
                        {
                            audience,
                            "account",  // Default Keycloak audience
                            "api://default"
                        },

                        ValidIssuers = new[] { authority },

                        // Keycloak claim mappings
                        NameClaimType = "preferred_username",
                        RoleClaimType = "realm_access.roles",

                        ClockSkew = TimeSpan.FromMinutes(5)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILogger<JwtBearerEvents>>();

                            logger.LogError("JWT validation failed: {Message}",
                                context.Exception.Message);

                            logger.LogDebug("Full exception: {Exception}",
                                context.Exception.ToString());

                            return Task.CompletedTask;
                        },

                        OnTokenValidated = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILogger<JwtBearerEvents>>();

                            var username = context.Principal?.Identity?.Name ?? "Unknown";
                            logger.LogInformation("Token validated for user: {Username}", username);

                            var permissionsClaim = context.Principal?.FindFirst("authorization")?.Value;
                            if (!string.IsNullOrEmpty(permissionsClaim))
                            {
                                logger.LogDebug("User has permissions claim: {Permissions}",
                                    permissionsClaim);
                            }

                            return Task.CompletedTask;
                        },

                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                // Basic authenticated user policy
                options.AddPolicy("RequireAuthenticatedUser", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });

                // Role-based policies
                options.AddPolicy("RequireAdminRole", policy =>
                {
                    policy.RequireRole("admin");
                });

                options.AddPolicy("RequireManagerRole", policy =>
                {
                    policy.RequireRole("admin", "manager");
                });

                options.AddPolicy("RequireUserRole", policy =>
                {
                    policy.RequireRole("admin", "manager", "user");
                });
            });

            return services;
        }
    }
}