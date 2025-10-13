using GameNest.CatalogService.Api.Middlewares;
using GameNest.CatalogService.BLL.Consumers.Genres;
using GameNest.CatalogService.BLL.Extensions;
using GameNest.CatalogService.BLL.MappingProfiles;
using GameNest.CatalogService.BLL.Services;
using GameNest.CatalogService.BLL.Services.Interfaces;
using GameNest.CatalogService.BLL.Validators.Games;
using GameNest.CatalogService.DAL.Data;
using GameNest.CatalogService.DAL.Data.Seed;
using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.DAL.Repositories;
using GameNest.CatalogService.DAL.Repositories.Interfaces;
using GameNest.CatalogService.DAL.UOW;
using GameNest.ServiceDefaults.Extensions;
using GameNest.ServiceDefaults.Redis;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddOpenTelemetryTracing();
builder.Services.AddCorrelationIdForwarding();

var connectionString = builder.Configuration.GetConnectionString("gamenest-catalogservice-db")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(connectionString!));

builder.Services.AddRedisCache(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GenreUpdatedEventConsumer>();
    x.AddConsumer<GenreDeletedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton(provider =>
{
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    var config = AutoMapperConfig.RegisterMappings(loggerFactory);
    return config.CreateMapper();
});

builder.Services.AddFluentValidationSetup(typeof(GameCreateDtoValidator).Assembly);

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IDeveloperRepository, DeveloperRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IGameDeveloperRoleRepository, GameDeveloperRoleRepository>();
builder.Services.AddScoped<IGameGenreRepository, GameGenreRepository>();
builder.Services.AddScoped<IGamePlatformRepository, GamePlatformRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(ISortHelper<>), typeof(SortHelper<>));

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IDeveloperService, DeveloperService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IGamePlatformService, GamePlatformService>();
builder.Services.AddScoped<IGameGenreService, GameGenreService>();
builder.Services.AddScoped<IGameDeveloperRoleService, GameDeveloperRoleService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.MigrateAsync();
    await CatalogDbSeeder.SeedAsync(db); 
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCorrelationId();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();