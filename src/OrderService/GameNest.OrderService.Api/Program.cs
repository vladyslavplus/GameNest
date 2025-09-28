using GameNest.OrderService.Api.Extensions;
using GameNest.OrderService.Api.Middlewares;
using GameNest.OrderService.BLL.Mappings;
using GameNest.OrderService.BLL.Services;
using GameNest.OrderService.BLL.Services.Interfaces;
using GameNest.OrderService.DAL.Repositories;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using GameNest.OrderService.DAL.UOW;
using Npgsql;
using System.Data;
using DALConnection = GameNest.OrderService.DAL.Infrastructure.IConnectionFactory;
using DALConnectionImpl = GameNest.OrderService.DAL.Infrastructure.ConnectionFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(provider =>
{
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    var config = AutoMapperConfig.RegisterMappings(loggerFactory);
    return config.CreateMapper(); 
});

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Host.UseSerilogWithConfiguration();

var connectionString = builder.Configuration.GetConnectionString("gamenest-orderservice-db")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<IDbConnection>(provider =>
{
    var connection = new NpgsqlConnection(connectionString);
    connection.Open();
    return connection;
});

builder.Services.AddSingleton<DALConnection, DALConnectionImpl>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRecordRepository, PaymentRecordRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepositoryAdo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentRecordService, PaymentRecordService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
