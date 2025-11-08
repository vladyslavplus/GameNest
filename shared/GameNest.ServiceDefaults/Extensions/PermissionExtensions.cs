using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace GameNest.ServiceDefaults.Extensions
{
    /// <summary>
    /// Custom authorization attribute for permission-based access control.
    /// Usage: [RequirePermission("orders:create")]
    /// </summary>
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission)
        {
            Policy = $"Permission:{permission}";
        }
    }

    /// <summary>
    /// Authorization requirement for permission-based access control.
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        }
    }

    /// <summary>
    /// Authorization handler that validates permissions from Keycloak JWT tokens.
    /// Checks the "scope" claim which contains space-separated permissions.
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Extract the scope claim from JWT token
            // Keycloak includes permissions in the "scope" claim as space-separated values
            var scopeClaim = context.User.FindFirst("scope")?.Value;

            if (string.IsNullOrEmpty(scopeClaim))
            {
                // No scope claim found - access denied
                return Task.CompletedTask;
            }

            // Split scopes by space
            var scopes = scopeClaim.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Check if user has the required permission
            if (scopes.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Extension methods for configuring permission-based authorization.
    /// </summary>
    public static class PermissionExtensions
    {
        /// <summary>
        /// Adds permission-based authorization to the service collection.
        /// This enables the use of [RequirePermission] attribute on controllers/endpoints.
        /// </summary>
        public static IServiceCollection AddPermissionAuthorization(
            this IServiceCollection services)
        {
            // Register the permission handler
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // Add authorization policies for common permissions
            services.AddAuthorizationBuilder()

                // ===== CATALOG PERMISSIONS =====
                .AddPolicy("Permission:catalog:write", policy =>
                    policy.Requirements.Add(new PermissionRequirement("catalog:write")))
                .AddPolicy("Permission:catalog:delete", policy =>
                    policy.Requirements.Add(new PermissionRequirement("catalog:delete")))

                // ===== ORDER PERMISSIONS =====
                .AddPolicy("Permission:orders:read", policy =>
                    policy.Requirements.Add(new PermissionRequirement("orders:read")))
                .AddPolicy("Permission:orders:create", policy =>
                    policy.Requirements.Add(new PermissionRequirement("orders:create")))
                .AddPolicy("Permission:orders:update", policy =>
                    policy.Requirements.Add(new PermissionRequirement("orders:update")))
                .AddPolicy("Permission:orders:delete", policy =>
                    policy.Requirements.Add(new PermissionRequirement("orders:delete")))

                // ===== PAYMENT PERMISSIONS =====
                .AddPolicy("Permission:payments:read", policy =>
                    policy.Requirements.Add(new PermissionRequirement("payments:read")))
                .AddPolicy("Permission:payments:create", policy =>
                    policy.Requirements.Add(new PermissionRequirement("payments:create")))
                .AddPolicy("Permission:payments:update", policy =>
                    policy.Requirements.Add(new PermissionRequirement("payments:update")))
                .AddPolicy("Permission:payments:delete", policy =>
                    policy.Requirements.Add(new PermissionRequirement("payments:delete")))

                // ===== REVIEW PERMISSIONS =====
                .AddPolicy("Permission:reviews:write", policy =>
                    policy.Requirements.Add(new PermissionRequirement("reviews:write")))
                .AddPolicy("Permission:reviews:update", policy =>
                    policy.Requirements.Add(new PermissionRequirement("reviews:write")))
                .AddPolicy("Permission:reviews:delete", policy =>
                    policy.Requirements.Add(new PermissionRequirement("reviews:delete")));

            return services;
        }

        /// <summary>
        /// Adds a custom permission policy dynamically.
        /// Useful for runtime policy registration.
        /// </summary>
        public static IServiceCollection AddPermissionPolicy(
            this IServiceCollection services,
            string permission)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy($"Permission:{permission}", policy =>
                    policy.Requirements.Add(new PermissionRequirement(permission)));

            return services;
        }
    }
}