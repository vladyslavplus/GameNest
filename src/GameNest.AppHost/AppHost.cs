var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithBindMount("sql", "/docker-entrypoint-initdb.d") 
    .WithPgAdmin();

var mongo = builder.AddMongoDB("mongodb")
    .WithDataVolume();

var mongoDb = mongo.AddDatabase("gamenest-reviewservice-db");

var ordersDb = postgres.AddDatabase("gamenest-orderservice-db");

builder.AddProject<Projects.GameNest_OrderService_Api>("orderservice-api")
    .WithReference(ordersDb)
    .WaitFor(ordersDb);

var catalogDb = postgres.AddDatabase("gamenest-catalogservice-db");

builder.AddProject<Projects.GameNest_CatalogService_Api>("catalogservice-api")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

builder.AddProject<Projects.GameNest_ReviewsService_Api>("reviewsservice-api")
    .WithReference(mongoDb)
    .WaitFor(mongoDb);

await builder.Build().RunAsync();