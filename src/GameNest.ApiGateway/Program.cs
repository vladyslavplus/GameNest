using GameNest.ApiGateway.Extensions;
using GameNest.ServiceDefaults.Extensions;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.AddServiceDefaults();
builder.AddOpenTelemetryTracing();

builder.Services.AddServiceDiscovery();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver()
    .AddTransforms(context =>
    {
        context.AddRequestTransform(async transformContext =>
        {
            var correlationId = transformContext.HttpContext.Items["X-Correlation-Id"]?.ToString();
            if (!string.IsNullOrEmpty(correlationId))
            {
                transformContext.ProxyRequest.Headers.TryAddWithoutValidation(
                    "X-Correlation-Id",
                    correlationId);
            }
            await ValueTask.CompletedTask;
        });
    });

builder.Services.AddCorrelationIdForwarding();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseGatewayPipeline();
app.MapReverseProxy();
app.MapHealthChecks("/health");
await app.RunAsync();