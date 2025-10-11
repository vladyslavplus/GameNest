using GameNest.AggregatorService.Clients;
using GameNest.AggregatorService.Services;
using GameNest.ServiceDefaults.Extensions;
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

builder.Services.AddCorrelationIdHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalogservice-api");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(10);
})
.AddServiceDiscovery();

builder.Services.AddCorrelationIdHttpClient<ReviewsClient>(client =>
{
    client.BaseAddress = new Uri("http://reviewsservice-api");
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