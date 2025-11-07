using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using GameNest.IdentityServerService.Api.Data;
using GameNest.IdentityServerService.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameNest.IdentityServerService.Api;

public static class SeedData
{
    public static async Task EnsureSeedDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var identityDb = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await identityDb.Database.MigrateAsync();

        var persistedGrantDb = serviceProvider.GetRequiredService<PersistedGrantDbContext>();
        await persistedGrantDb.Database.MigrateAsync();

        var configDb = serviceProvider.GetRequiredService<ConfigurationDbContext>();
        await configDb.Database.MigrateAsync();

        if (!configDb.Clients.Any())
        {
            foreach (var client in Config.Clients)
                configDb.Clients.Add(client.ToEntity());
            await configDb.SaveChangesAsync();
        }

        if (!configDb.IdentityResources.Any())
        {
            foreach (var resource in Config.IdentityResources)
                configDb.IdentityResources.Add(resource.ToEntity());
            await configDb.SaveChangesAsync();
        }

        if (!configDb.ApiScopes.Any())
        {
            foreach (var scopeEntity in Config.ApiScopes)
                configDb.ApiScopes.Add(scopeEntity.ToEntity());
            await configDb.SaveChangesAsync();
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole<Guid>("User"));

        var adminEmail = "admin@admin.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "Admin",
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin@1234");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        var userEmail = "user@user.com";
        var user = await userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "User",
                Email = userEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, "User@1234");
            await userManager.AddToRoleAsync(user, "User");
        }
    }
}