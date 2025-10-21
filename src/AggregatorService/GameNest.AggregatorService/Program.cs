using GameNest.AggregatorService.Clients;
using GameNest.AggregatorService.HealthChecks;
using GameNest.AggregatorService.Services;
using GameNest.Grpc.Games;
using GameNest.Grpc.OrderItems;
using GameNest.Grpc.Orders;
using GameNest.Grpc.Reviews;
using GameNest.ServiceDefaults.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddOpenTelemetryTracing();
builder.Services.AddCorrelationIdForwarding();
builder.Services.AddGrpcWithObservability(builder.Environment);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServiceDiscovery();

builder.Services.AddGrpcClient<GameGrpcService.GameGrpcServiceClient>(options =>
{
    options.Address = new Uri("https://catalogservice-api");
})
.ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
})
.AddServiceDiscovery()
.AddGrpcResilienceHandler(ResilienceProfile.Standard);

builder.Services.AddGrpcClient<ReviewGrpcService.ReviewGrpcServiceClient>(options =>
{
    options.Address = new Uri("https://reviewsservice-api");
})
.ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
})
.AddServiceDiscovery()
.AddGrpcResilienceHandler(ResilienceProfile.Standard);

builder.Services.AddGrpcClient<OrderGrpcService.OrderGrpcServiceClient>(options =>
{
    options.Address = new Uri("https://orderservice-api");
})
.ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
})
.AddServiceDiscovery()
.AddGrpcResilienceHandler(ResilienceProfile.Critical);

builder.Services.AddGrpcClient<OrderItemGrpcService.OrderItemGrpcServiceClient>(options =>
{
    options.Address = new Uri("https://orderservice-api");
})
.ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
})
.AddServiceDiscovery()
.AddGrpcResilienceHandler(ResilienceProfile.Critical);

builder.Services.AddScoped<CatalogGrpcClient>();
builder.Services.AddScoped<ReviewsGrpcClient>();
builder.Services.AddScoped<OrdersGrpcClient>();
builder.Services.AddScoped<OrderItemsGrpcClient>();

builder.Services.AddTransient<OrderAggregatorService>();
builder.Services.AddTransient<GameAggregatorService>();

builder.Services.AddCorrelationIdForwarding();

builder.Services.AddHealthChecks()
    .AddCheck<CatalogServiceHealthCheck>(
        "catalog-grpc",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "grpc", "downstream", "ready" })
    .AddCheck<ReviewsServiceHealthCheck>(
        "reviews-grpc",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "grpc", "downstream", "ready" })
    .AddCheck<OrdersServiceHealthCheck>(
        "orders-grpc",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "grpc", "downstream", "ready", "critical" })
    .AddCheck<OrderItemsServiceHealthCheck>(
        "orderitems-grpc",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "grpc", "downstream", "ready", "critical" });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCorrelationId();
app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();