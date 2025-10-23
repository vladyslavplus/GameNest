using GameNest.IdentityService.BLL.Configuration;
using GameNest.IdentityService.BLL.Interfaces;
using GameNest.IdentityService.BLL.MappingProfiles;
using GameNest.IdentityService.BLL.Services;
using GameNest.IdentityService.DAL.Data;
using GameNest.IdentityService.DAL.Interfaces;
using GameNest.IdentityService.DAL.Repositories;
using GameNest.IdentityService.Domain.Entities;
using GameNest.ServiceDefaults.Extensions;
using GameNest.ServiceDefaults.Health;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddOpenTelemetryTracing();
builder.Services.AddCorrelationIdForwarding();

var connectionString = builder.Configuration.GetConnectionString("gamenest-identityservice-db")
                    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(connectionString!));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAutoMapperWithLogging(typeof(IdentityProfile).Assembly);

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuth("GameNest Identity API");

builder.Services.AddHealthChecks()
    .AddPostgresHealthCheck(
        configuration: builder.Configuration,
        connectionName: "gamenest-identityservice-db",
        serviceName: "identityservice",
        timeoutSeconds: 5);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await dbContext.Database.MigrateAsync();
    await IdentitySeeder.SeedAsync(app.Services);
}

app.UseSwaggerInDevelopment();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCorrelationId();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

await app.RunAsync();
