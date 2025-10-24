using GameNest.Grpc.Carts;
using GameNest.OrderService.Api.Middlewares;
using GameNest.OrderService.BLL.Mappings;
using GameNest.OrderService.BLL.Services;
using GameNest.OrderService.BLL.Services.Interfaces;
using GameNest.OrderService.DAL.Repositories;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using GameNest.OrderService.DAL.UOW;
using GameNest.OrderService.GrpcClients.Clients;
using GameNest.OrderService.GrpcClients.Clients.Interfaces;
using GameNest.OrderService.GrpcServer.MappingProfiles;
using GameNest.OrderService.GrpcServer.Services;
using GameNest.ServiceDefaults.Extensions;
using GameNest.ServiceDefaults.Health;
using MassTransit;
using Npgsql;
using System.Data;
using DALConnection = GameNest.OrderService.DAL.Infrastructure.IConnectionFactory;
using DALConnectionImpl = GameNest.OrderService.DAL.Infrastructure.ConnectionFactory;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddOpenTelemetryTracing();
builder.Services.AddCorrelationIdForwarding();
builder.Services.AddGrpcWithObservability(builder.Environment);
builder.Services.AddServiceDiscovery();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAutoMapperWithLogging(
    typeof(OrderProfile).Assembly,
    typeof(OrderGrpcProfile).Assembly
);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var connectionString = builder.Configuration.GetConnectionString("gamenest-orderservice-db")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<IDbConnection>(provider =>
{
    var connection = new NpgsqlConnection(connectionString);
    connection.Open();
    return connection;
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddGrpcClient<CartGrpcService.CartGrpcServiceClient>(options =>
{
    options.Address = new Uri("https://cartservice-api");
})
.ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
})
.AddServiceDiscovery()
.AddGrpcResilienceHandler(ResilienceProfile.Standard);

builder.Services.AddSingleton<DALConnection, DALConnectionImpl>();

builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRecordRepository, PaymentRecordRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentRecordService, PaymentRecordService>();
builder.Services.AddScoped<ICartGrpcClient, CartGrpcClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuth("GameNest Order API");

builder.Services
    .AddHealthChecks()
    .AddPostgresHealthCheck(
        configuration: builder.Configuration,
        connectionName: "gamenest-orderservice-db",
        serviceName: "orderservice"
    );

var app = builder.Build();

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
app.MapGrpcService<OrderGrpcServiceImpl>();
app.MapGrpcService<OrderItemGrpcServiceImpl>();
app.MapGrpcServicesWithReflection();

await app.RunAsync();
