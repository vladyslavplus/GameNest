using GameNest.IdentityService.Domain.Entities;
using GameNest.Shared.TestData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GameNest.IdentityService.DAL.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");
            var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await context.Database.MigrateAsync();

            string[] roles = ["Admin", "User"];
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                    logger.LogInformation("Created role {Role}", roleName);
                }
            }

            var adminId = Guid.Parse(SharedSeedData.Users.Admin);
            var adminEmail = "admin@admin.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    Id = adminId,
                    UserName = "Admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin, "Admin@1234");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    logger.LogInformation("Created admin user: {Email}", adminEmail);
                }
                else
                {
                    logger.LogWarning("Failed to create admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            var testUsers = new List<(Guid Id, string Email, string Username)>
            {
                (Guid.Parse(SharedSeedData.Users.JohnDoe), "john@gamenest.com", "JohnDoe"),
                (Guid.Parse(SharedSeedData.Users.AliceWonder), "alice@gamenest.com", "AliceWonder"),
                (Guid.Parse(SharedSeedData.Users.MarkSmith), "mark@gamenest.com", "MarkSmith")
            };

            foreach (var (id, email, username) in testUsers)
            {
                if (await userManager.FindByEmailAsync(email) is not null)
                    continue;

                var user = new ApplicationUser
                {
                    Id = id,
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, "User@1234");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    logger.LogInformation("Created test user: {Email}", email);
                }
                else
                {
                    logger.LogWarning("Failed to create test user {Email}: {Errors}", email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
