var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithBindMount("sql", "/docker-entrypoint-initdb.d") 
    .WithPgAdmin();

var ordersDb = postgres.AddDatabase("gamenest-orderservice-db");

builder.AddProject<Projects.GameNest_OrderService_Api>("orderservice-api")
    .WithReference(ordersDb)
    .WaitFor(ordersDb);

await builder.Build().RunAsync();