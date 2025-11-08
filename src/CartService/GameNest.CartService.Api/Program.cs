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

builder.Services
    .AddKeycloakAuthentication(builder.Configuration)
    .AddAuthorization();

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
builder.Services.AddSwaggerWithKeycloak(builder.Configuration, "GameNest Cart API");

builder.Services.AddHealthChecks()
    .AddRedis(
        builder.Configuration.GetConnectionString("redis")
            ?? builder.Configuration.GetConnectionString("Redis")!,
        name: "cartservice-redis-check",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "redis", "ready" });

var app = builder.Build();

app.UseSwaggerWithKeycloak();

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