using GameNest.CartService.Api.Middlewares;
using GameNest.CartService.BLL.Interfaces;
using GameNest.CartService.BLL.MappingProfiles;
using GameNest.CartService.BLL.Services;
using GameNest.CartService.DAL.Interfaces;
using GameNest.CartService.DAL.Repositories;
using GameNest.ServiceDefaults.Extensions;
using GameNest.ServiceDefaults.Redis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddOpenTelemetryTracing();
builder.Services.AddCorrelationIdForwarding();
builder.Services.AddGrpcWithObservability(builder.Environment);

builder.Services.AddSingleton(provider =>
{
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    var config = AutoMapperConfig.RegisterMappings(loggerFactory);
    return config.CreateMapper();
});

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddRedisCache(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddRedis(
        builder.Configuration.GetConnectionString("redis")
            ?? builder.Configuration.GetConnectionString("Redis")!,
        name: "cartservice-redis-check",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "redis", "ready" });

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
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseCorrelationId();
app.UseRouting();

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.MapGrpcServicesWithReflection();

await app.RunAsync();
