using GameNest.OrderService.Api.Extensions;
using Npgsql;
using System.Data;
using DALConnection = GameNest.OrderService.DAL.Infrastructure.IConnectionFactory;
using DALConnectionImpl = GameNest.OrderService.DAL.Infrastructure.ConnectionFactory;

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Host.UseSerilogWithConfiguration();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));
builder.Services.AddSingleton<DALConnection, DALConnectionImpl>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
