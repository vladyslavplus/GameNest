using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace GameNest.ServiceDefaults.Extensions
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var authority = configuration["Identity__Authority"]
                          ?? configuration["Identity:Authority"]
                          ?? "https://localhost:7052";

            var audience = configuration["Identity__Audience"]
                        ?? configuration["Identity:Audience"]
                        ?? "gamenest_api";

            if (string.IsNullOrWhiteSpace(authority))
            {
                throw new InvalidOperationException(
                    "JWT Authority is not configured. Please set 'Identity:Authority' in appsettings.json " +
                    "or 'Identity__Authority' as environment variable.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.Audience = audience;
                    options.RequireHttpsMetadata = false;

                    options.RefreshOnIssuerKeyNotFound = true;

                    options.TokenValidationParameters = new()
                    {
                        ValidAudiences = new[]
                        {
                            "gamenest_api",
                            "https://localhost:7052/resources"
                        },
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RoleClaimType = "role",
                        NameClaimType = "name",
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILogger<JwtBearerEvents>>();
                            logger.LogError("JWT validation failed: {Message}", context.Exception.Message);

                            if (context.Exception is SecurityTokenSignatureKeyNotFoundException)
                            {
                                logger.LogWarning("Signing key not found. Triggering configuration refresh...");
                                if (options.ConfigurationManager != null)
                                    options.ConfigurationManager.RequestRefresh();
                            }

                            logger.LogDebug("Full exception: {Exception}", context.Exception.ToString());
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILogger<JwtBearerEvents>>();
                            logger.LogInformation("Token validated successfully for user: {Name}",
                                context.Principal?.Identity?.Name ?? "Unknown");
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAuthenticatedUser", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "gamenest_api");
                });
            });

            return services;
        }
    }
}