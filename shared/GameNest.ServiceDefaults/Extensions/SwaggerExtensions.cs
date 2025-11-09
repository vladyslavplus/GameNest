using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace GameNest.ServiceDefaults.Extensions
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Adds Swagger with Keycloak OAuth2 authentication support.
        /// </summary>
        public static IServiceCollection AddSwaggerWithKeycloak(
            this IServiceCollection services,
            IConfiguration configuration,
            string title,
            string version = "v1")
        {
            var keycloakUrl = configuration["Keycloak:Url"]
                           ?? configuration["services:keycloak:http:0"]
                           ?? "http://localhost:8080";

            var realm = configuration["Keycloak:Realm"] ?? "GameNest";

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = title,
                    Version = version,
                    Description = $"GameNest API authenticated via Keycloak (Realm: {realm})",
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "OAuth2 Authorization Code Flow with PKCE via Keycloak",
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{keycloakUrl}/realms/{realm}/protocol/openid-connect/auth"),
                            TokenUrl = new Uri($"{keycloakUrl}/realms/{realm}/protocol/openid-connect/token"),

                            Scopes = new Dictionary<string, string>
                            {
                                // OpenID Connect standard scopes
                                { "openid", "OpenID Connect scope" },
                                { "profile", "User profile information" },
                                { "email", "User email address" },

                                // GameNest API access
                                { "gamenest_api", "Full access to GameNest APIs" },

                                { "catalog:write", "Write to catalog" },
                                { "catalog:delete", "Delete from catalog" },

                                { "orders:read", "Read user's orders" },
                                { "orders:create", "Create new orders" },
                                { "orders:update", "Update existing orders" },
                                { "orders:delete", "Delete existing orders" },

                                { "payments:read", "Read payment records" },
                                { "payments:create", "Create payment records" },
                                { "payments:update", "Update payment records" },
                                { "payments:delete", "Delete payment records" },

                                { "reviews:write", "Create new reviews" },
                                { "reviews:update", "Update reviews or comments" },
                                { "reviews:delete", "Delete reviews" },
                            }
                        }
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },
                        new[]
                        {
                            "openid",
                            "profile",
                            "gamenest_api",
                            "catalog:write",
                            "catalog:delete",
                            "orders:read",
                            "orders:create",
                            "orders:update",
                            "orders:delete",
                            "payments:read",
                            "payments:create",
                            "payments:update",
                            "payments:delete",
                            "reviews:write",
                            "reviews:update",
                            "reviews:delete"
                        }
                    }
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        /// <summary>
        /// Configures Swagger UI with Keycloak OAuth2 settings.
        /// Only enables Swagger in Development environment.
        /// </summary>
        public static WebApplication UseSwaggerWithKeycloak(
            this WebApplication app,
            IConfiguration? configuration = null)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json",
                        $"{app.Environment.ApplicationName} v1");

                    options.OAuthClientId("swagger");
                    options.OAuthAppName("GameNest Swagger UI");
                    options.OAuthUsePkce();

                    options.OAuthScopes(
                        "openid",
                        "profile",
                        "email",
                        "gamenest_api"
                    );

                    options.OAuthScopeSeparator(" ");

                    // Optional: Persist authorization data
                    options.EnablePersistAuthorization();

                    options.DisplayRequestDuration();
                });
            }

            return app;
        }
    }
}