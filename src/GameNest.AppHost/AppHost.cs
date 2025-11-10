using GameNest.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("PGSSLMODE", "disable")
    .WithDataVolume()
    .WithBindMount("sql", "/docker-entrypoint-initdb.d")
    .WithPgAdmin();

var mongo = builder.AddMongoDB("mongodb")
    .WithDataVolume();

var mongoDb = mongo.AddDatabase("gamenest-reviewservice-db");

var ordersDb = postgres.AddDatabase("gamenest-orderservice-db");
var catalogDb = postgres.AddDatabase("gamenest-catalogservice-db");
// Keep old identity databases for reference but don't use them
// var identityDb = postgres.AddDatabase("gamenest-identityservice-db");
// var identityServerDb = postgres.AddDatabase("gamenest-identityserverservice-db");

var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithRedisCommander();

var rabbitmq = builder.AddRabbitMQ("rabbitmq",
        userName: builder.AddParameter("username", "admin", secret: true),
        password: builder.AddParameter("password", "admin123", secret: true))
    .WithManagementPlugin()
    .WithDataVolume();

// ===== OLD IDENTITY SERVICES (DISABLED - kept for reference) =====
/*
var identityService = builder.AddProject<Projects.GameNest_IdentityService_Api>("identityservice-api")
    .WithReference(identityDb)
    .WaitFor(identityDb)
    .WithHttpEndpoint(port: 5006, name: "identity-http")
    .WithHttpsEndpoint(port: 7051, name: "identity-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var identityServerService = builder.AddProject<Projects.GameNest_IdentityServerService_Api>("identityserverservice-api")
    .WithReference(identityServerDb)
    .WaitFor(identityServerDb)
    .WithHttpEndpoint(port: 5007, name: "identityserver-http")
    .WithHttpsEndpoint(port: 7052, name: "identityserver-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);
*/

var keycloakAdminUser = builder.AddParameter("keycloak-admin-username", "admin", secret: true);
var keycloakAdminPass = builder.AddParameter("keycloak-admin-password", "admin", secret: true);

var keycloak = builder.AddKeycloak("keycloak", port: 8080, keycloakAdminUser, keycloakAdminPass)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithAutoConfiguration();

var catalogService = builder.AddProject<Projects.GameNest_CatalogService_Api>("catalogservice-api")
    .WithReference(catalogDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(keycloak)
    .WaitFor(catalogDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak)
    .WithHttpEndpoint(port: 5002, name: "catalog-http")
    .WithHttpsEndpoint(port: 7048, name: "catalog-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithHttpHealthCheck("/health");

var reviewsService = builder.AddProject<Projects.GameNest_ReviewsService_Api>("reviewsservice-api")
    .WithReference(mongoDb)
    .WithReference(catalogService)
    .WithReference(keycloak)
    .WaitFor(mongoDb)
    .WaitFor(catalogService)
    .WaitFor(keycloak)
    .WithHttpEndpoint(port: 5003, name: "reviews-http")
    .WithHttpsEndpoint(port: 7047, name: "reviews-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithEnvironment("Grpc__CatalogService", "https://catalogservice-api")
    .WithHttpHealthCheck("/health");

var cartService = builder.AddProject<Projects.GameNest_CartService_Api>("cartservice-api")
    .WithReference(redis)
    .WithReference(catalogService)
    .WithReference(rabbitmq)
    .WithReference(keycloak)
    .WaitFor(redis)
    .WaitFor(catalogService)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak)
    .WithHttpEndpoint(port: 5005, name: "cart-http")
    .WithHttpsEndpoint(port: 7050, name: "cart-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithEnvironment("Grpc__CatalogService", "https://catalogservice-api")
    .WithHttpHealthCheck("/health");

var orderservice = builder.AddProject<Projects.GameNest_OrderService_Api>("orderservice-api")
    .WithReference(ordersDb)
    .WithReference(cartService)
    .WithReference(rabbitmq)
    .WithReference(keycloak)
    .WaitFor(ordersDb)
    .WaitFor(cartService)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak)
    .WithHttpEndpoint(port: 5001, name: "orders-http")
    .WithHttpsEndpoint(port: 7045, name: "order-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithHttpHealthCheck("/health");

var aggregatorService = builder.AddProject<Projects.GameNest_AggregatorService>("aggregatorservice-api")
    .WithReference(orderservice)
    .WithReference(catalogService)
    .WithReference(reviewsService)
    .WithReference(cartService)
    .WithReference(keycloak)
    .WaitFor(orderservice)
    .WaitFor(catalogService)
    .WaitFor(reviewsService)
    .WaitFor(cartService)
    .WaitFor(keycloak)
    .WithHttpEndpoint(port: 5004, name: "aggregator-http")
    .WithHttpsEndpoint(port: 7049, name: "aggregator-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.GameNest_ApiGateway>("gateway")
    .WithReference(orderservice)
    .WithReference(catalogService)
    .WithReference(reviewsService)
    .WithReference(aggregatorService)
    .WithReference(cartService)
    .WithReference(keycloak)
    .WaitFor(orderservice)
    .WaitFor(catalogService)
    .WaitFor(reviewsService)
    .WaitFor(aggregatorService)
    .WaitFor(cartService)
    .WaitFor(keycloak)
    .WithHttpEndpoint(port: 5000, name: "gateway-http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithHttpHealthCheck("/health");

await builder.Build().RunAsync();