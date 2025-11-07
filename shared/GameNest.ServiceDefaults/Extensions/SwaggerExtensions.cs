using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace GameNest.ServiceDefaults.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerWithAuth(
            this IServiceCollection services,
            string title,
            string version = "v1")
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = title,
                    Version = version
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "OAuth2 Authorization Code Flow with PKCE via GameNest IdentityServer",
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:7052/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:7052/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID Connect scope" },
                                { "profile", "User profile information" },
                                { "gamenest_api", "Full access to all GameNest APIs" }
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
                        new[] { "openid", "profile", "gamenest_api" }
                    }
                });
            });

            return services;
        }

        public static WebApplication UseSwaggerInDevelopment(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", app.Environment.ApplicationName + " v1");
                    options.OAuthClientId("swagger");
                    options.OAuthAppName("GameNest Swagger UI");
                    options.OAuthUsePkce();
                    options.OAuthScopes("openid", "profile", "gamenest_api");
                    options.OAuthScopeSeparator(" ");
                });
            }

            return app;
        }
    }
}