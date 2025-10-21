using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace GameNest.ServiceDefaults.Extensions
{
    public static class TelemetryExtensions
    {
        public static IHostApplicationBuilder AddOpenTelemetryTracing(this IHostApplicationBuilder builder)
        {
            var serviceName = builder.Configuration["OTEL_SERVICE_NAME"]
                ?? builder.Environment.ApplicationName;
            var env = builder.Environment.EnvironmentName;

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService(serviceName: serviceName, serviceVersion: "1.0.0")
                    .AddTelemetrySdk()
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["environment"] = env
                    }))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;
                            options.EnrichWithHttpRequest = (activity, request) =>
                            {
                                if (request.HttpContext.Items.TryGetValue("X-Correlation-Id", out var correlationId))
                                    activity.SetTag("correlation.id", correlationId);
                            };
                        })
                        .AddGrpcClientInstrumentation(options =>
                        {
                            options.EnrichWithHttpRequestMessage = (activity, req) =>
                            {
                                activity.SetTag("rpc.system", "grpc");
                                activity.SetTag("rpc.service", req.RequestUri?.Segments.SkipLast(1).LastOrDefault()?.Trim('/'));
                                activity.SetTag("rpc.method", req.RequestUri?.Segments.LastOrDefault()?.Trim('/'));
                                activity.SetTag("net.peer.name", req.RequestUri?.Host);
                                activity.SetTag("net.peer.port", req.RequestUri?.Port);
                            };
                            options.EnrichWithHttpResponseMessage = (activity, res) =>
                            {
                                activity.SetTag("rpc.grpc.status_code", (int)res.StatusCode);
                            };
                        })
                        .AddSource("Grpc.AspNetCore.Server")
                        .AddSource("Grpc.Net.Client")
                        .AddHttpClientInstrumentation(options =>
                        {
                            options.RecordException = true;
                        })
                        .AddEntityFrameworkCoreInstrumentation(options =>
                        {
                            options.SetDbStatementForText = true;
                            options.EnrichWithIDbCommand = (activity, command) =>
                            {
                                activity?.SetTag("db.commandTimeout", command.CommandTimeout);
                            };
                        })
                        .AddSource("MongoDb.Driver")
                        .AddSource("Yarp.ReverseProxy")
                        .AddOtlpExporter();

                    tracing.SetSampler(env == "Development"
                        ? new AlwaysOnSampler()
                        : new TraceIdRatioBasedSampler(0.2));
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddMeter("Yarp.ReverseProxy")
                        .AddMeter("GameNest.Cache")
                        .AddMeter("GameNest.CatalogService.Games")
                        .AddOtlpExporter();
                });

            return builder;
        }
    }
}