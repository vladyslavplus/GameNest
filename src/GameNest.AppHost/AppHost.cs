var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithBindMount("sql", "/docker-entrypoint-initdb.d")
    .WithPgAdmin();

var mongo = builder.AddMongoDB("mongodb")
    .WithDataVolume();

var mongoDb = mongo.AddDatabase("gamenest-reviewservice-db");

var ordersDb = postgres.AddDatabase("gamenest-orderservice-db");
var catalogDb = postgres.AddDatabase("gamenest-catalogservice-db");

var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithRedisCommander();

var rabbitmq = builder.AddRabbitMQ("rabbitmq",
        userName: builder.AddParameter("username", "admin", secret: true),
        password: builder.AddParameter("password", "admin123", secret: true))
    .WithManagementPlugin()
    .WithDataVolume();

var orderservice = builder.AddProject<Projects.GameNest_OrderService_Api>("orderservice-api")
    .WithReference(ordersDb)
    .WaitFor(ordersDb)
    .WithHttpEndpoint(port: 5001, name: "orders-http")
    .WithHttpsEndpoint(port: 7045, name: "order-https")
    .WithHttpHealthCheck("/health")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var catalogService = builder.AddProject<Projects.GameNest_CatalogService_Api>("catalogservice-api")
    .WithReference(catalogDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(catalogDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq)
    .WithHttpEndpoint(port: 5002, name: "catalog-http")
    .WithHttpsEndpoint(port: 7048, name: "catalog-https")
    .WithHttpHealthCheck("/health")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var reviewsService = builder.AddProject<Projects.GameNest_ReviewsService_Api>("reviewsservice-api")
    .WithReference(mongoDb)
    .WaitFor(mongoDb)
    .WithHttpEndpoint(port: 5003, name: "reviews-http")
    .WithHttpsEndpoint(port: 7047, name: "reviews-https")
    .WithHttpHealthCheck("/health")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var aggregatorService = builder.AddProject<Projects.GameNest_AggregatorService>("aggregatorservice-api")
    .WithReference(orderservice)
    .WithReference(catalogService)
    .WithReference(reviewsService)
    .WaitFor(orderservice)
    .WaitFor(catalogService)
    .WaitFor(reviewsService)
    .WithHttpEndpoint(port: 5004, name: "aggregator-http")
    .WithHttpsEndpoint(port: 7049, name: "aggregator-https")
    .WithHttpHealthCheck("/health")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

builder.AddProject<Projects.GameNest_ApiGateway>("gateway")
    .WithReference(orderservice)
    .WithReference(catalogService)
    .WithReference(reviewsService)
    .WithReference(aggregatorService)
    .WaitFor(orderservice)
    .WaitFor(catalogService)
    .WaitFor(reviewsService)
    .WaitFor(aggregatorService)
    .WithHttpEndpoint(port: 5000, name: "gateway-http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithHttpHealthCheck("/health");

await builder.Build().RunAsync();