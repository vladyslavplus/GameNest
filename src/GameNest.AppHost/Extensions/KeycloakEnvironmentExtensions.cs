namespace GameNest.AppHost.Extensions
{
    public static class KeycloakEnvironmentExtensions
    {
        public static IResourceBuilder<T> WithKeycloakEnvironment<T>(
            this IResourceBuilder<T> builder,
            EndpointReference keycloakEndpoint,
            string realm,
            string audience)
            where T : IResourceWithEnvironment
        {
            return builder
                .WithEnvironment("Keycloak__Url", keycloakEndpoint)
                .WithEnvironment("Keycloak__Realm", realm)
                .WithEnvironment("Keycloak__Audience", audience);
        }
    }
}