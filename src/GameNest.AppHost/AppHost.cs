var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithBindMount("sql", "/docker-entrypoint-initdb.d") 
    .WithPgAdmin();

var ordersDb = postgres.AddDatabase("gamenest-orderservice-db");

builder.AddProject<Projects.GameNest_OrderService_Api>("orderservice-api")
    .WithReference(ordersDb)
    .WaitFor(ordersDb);

var catalogDb = postgres.AddDatabase("gamenest-catalogservice-db");

builder.AddProject<Projects.GameNest_CatalogService_Api>("catalogservice-api")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

await builder.Build().RunAsync();