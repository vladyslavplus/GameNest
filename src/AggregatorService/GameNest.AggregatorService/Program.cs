using GameNest.AggregatorService.Clients;
using GameNest.AggregatorService.Services;
using GameNest.ServiceDefaults.Extensions;
using GameNest.Grpc.Games;
using GameNest.Grpc.Reviews;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddOpenTelemetryTracing();

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
.AddServiceDiscovery();

builder.Services.AddGrpcClient<ReviewGrpcService.ReviewGrpcServiceClient>(options =>
{
    options.Address = new Uri("https://reviewsservice-api");
})
.ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
})
.AddServiceDiscovery();

builder.Services.AddScoped<CatalogGrpcClient>();
builder.Services.AddScoped<ReviewsGrpcClient>();

builder.Services.AddTransient<OrderAggregatorService>();
builder.Services.AddTransient<GameAggregatorService>();
builder.Services.AddCorrelationIdForwarding();

builder.Services.AddCorrelationIdHttpClient<OrdersClient>(client =>
{
    client.BaseAddress = new Uri("http://orderservice-api");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(5);
})
.AddServiceDiscovery();

builder.Services.AddCorrelationIdHttpClient<OrderItemsClient>(client =>
{
    client.BaseAddress = new Uri("http://orderservice-api");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(5);
})
.AddServiceDiscovery();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCorrelationId();
app.MapControllers();

await app.RunAsync();