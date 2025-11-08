using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameNest.ServiceDefaults.Extensions
{
    public static class KeycloakAuthenticationExtensions
    {
        /// <summary>
        /// Adds Keycloak JWT authentication for microservices.
        /// Uses configuration from appsettings or Aspire service discovery.
        /// </summary>
        public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var realm = configuration["Keycloak:Realm"] ?? "GameNest";
            var audience = configuration["Keycloak:Audience"] ?? "gamenest_api";
            var keycloakUrl = configuration["Keycloak:Url"]
                           ?? configuration["services:keycloak:http:0"]
                           ?? "http://localhost:8080";

            services
                .AddAuthentication()
                .AddKeycloakJwtBearer("keycloak",
                    realm: realm,
                    options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.Audience = audience;
                        options.Authority = keycloakUrl != null
                            ? $"{keycloakUrl.TrimEnd('/')}/realms/{realm}"
                            : $"http://localhost:8080/realms/{realm}";
                    });

            return services;
        }
    }
}
