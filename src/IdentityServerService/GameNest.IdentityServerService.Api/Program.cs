using GameNest.IdentityServerService.Api;
using GameNest.IdentityServerService.Api.Data;
using GameNest.IdentityServerService.Api.Entities;
using GameNest.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddOpenTelemetryTracing();
builder.Services.AddCorrelationIdForwarding();

var connectionString = builder.Configuration.GetConnectionString("gamenest-identityserverservice-db")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
{
    options.IssuerUri = "https://localhost:7052";
    options.EmitStaticAudienceClaim = true;
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
})
    .AddAspNetIdentity<ApplicationUser>()
    .AddConfigurationStore(opt =>
    {
        opt.ConfigureDbContext = db =>
            db.UseNpgsql(connectionString,
                npgsql => npgsql.MigrationsAssembly(migrationsAssembly));
    })
    .AddOperationalStore(opt =>
    {
        opt.ConfigureDbContext = db =>
            db.UseNpgsql(connectionString,
                npgsql => npgsql.MigrationsAssembly(migrationsAssembly));
        opt.EnableTokenCleanup = true;
        opt.TokenCleanupInterval = 3600;
    })
    .AddDeveloperSigningCredential(true, Path.Combine(AppContext.BaseDirectory, "tempkey.rsa"));



builder.Services.AddRazorPages();
// builder.Services.AddSwaggerWithAuth("GameNest IdentityServerService API");

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();

// app.UseSwaggerInDevelopment();

app.MapRazorPages();
await SeedData.EnsureSeedDataAsync(app.Services);
await app.RunAsync();