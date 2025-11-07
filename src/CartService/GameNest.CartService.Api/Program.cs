using GameNest.CartService.Api.Middlewares;
using GameNest.CartService.BLL.Consumers;
using GameNest.CartService.BLL.Interfaces;
using GameNest.CartService.BLL.MappingProfiles;
using GameNest.CartService.BLL.Services;
using GameNest.CartService.DAL.Interfaces;
using GameNest.CartService.DAL.Repositories;
using GameNest.CartService.GrpcServer.MappingProfiles;
using GameNest.CartService.GrpcServer.Services;
using GameNest.GrpcClients.Extensions;
using GameNest.ServiceDefaults.Extensions;
using GameNest.ServiceDefaults.Redis;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddOpenTelemetryTracing();
builder.Services.AddCorrelationIdForwarding();
builder.Services.AddGrpcWithObservability(builder.Environment);
builder.Services.AddServiceDiscovery();

var authority = builder.Configuration["Identity__Authority"]
             ?? builder.Configuration["Identity:Authority"]
             ?? "https://localhost:7052";

var audience = builder.Configuration["Identity__Audience"]
            ?? builder.Configuration["Identity:Audience"]
            ?? "gamenest_api";

Console.WriteLine($"[JWKS TEST] Authority (from any source) = '{authority}'");
Console.WriteLine($"[JWKS TEST] Audience (from any source) = '{audience}'");

Console.WriteLine("\n[CONFIG DUMP] All Identity configuration keys:");
foreach (var item in builder.Configuration.AsEnumerable().Where(x => x.Key.Contains("Identity", StringComparison.OrdinalIgnoreCase)))
{
    Console.WriteLine($"  {item.Key} = {item.Value}");
}
Console.WriteLine();

if (!string.IsNullOrWhiteSpace(authority))
{
    using var client = new HttpClient(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });

    try
    {
        var response = await client.GetAsync($"{authority}/.well-known/openid-configuration");
        var status = response.IsSuccessStatusCode ? "OK" : response.StatusCode.ToString();
        Console.WriteLine($"[JWKS TEST] Testing endpoint: {authority}/.well-known/openid-configuration");
        Console.WriteLine($"[JWKS TEST] Status: {status}");
        Console.WriteLine($"[JWKS TEST] Content Length: {(response.Content.Headers.ContentLength ?? 0)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[JWKS TEST] ERROR: {ex.Message}");
    }
}
else
{
    Console.WriteLine("[JWKS TEST] ERROR: Identity__Authority is null or empty.");
}

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAutoMapperWithLogging(
    typeof(CartProfile).Assembly,
    typeof(CartGrpcProfile).Assembly
);

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddRedisCache(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddGameGrpcClient(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuth("GameNest Cart API");

builder.Services.AddHealthChecks()
    .AddRedis(
        builder.Configuration.GetConnectionString("redis")
            ?? builder.Configuration.GetConnectionString("Redis")!,
        name: "cartservice-redis-check",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "redis", "ready" });

var app = builder.Build();

app.Logger.LogInformation("=== JWT AUTHENTICATION CONFIGURATION ===");
app.Logger.LogInformation($"Authority: {app.Configuration["Identity__Authority"]}");
app.Logger.LogInformation($"Audience: {app.Configuration["Identity__Audience"]}");

app.UseSwaggerInDevelopment();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseCorrelationId();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapGrpcService<CartGrpcServiceImpl>();
app.MapGrpcServicesWithReflection();

await app.RunAsync();