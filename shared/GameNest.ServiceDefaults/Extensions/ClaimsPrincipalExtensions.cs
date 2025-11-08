using System.Security.Claims;

namespace GameNest.ServiceDefaults.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the user's unique identifier from JWT token.
        /// Keycloak stores user ID in 'sub' claim as UUID.
        /// </summary>
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var subClaim = user.FindFirst("sub")
                           ?? user.FindFirst(ClaimTypes.NameIdentifier);

            if (subClaim == null || string.IsNullOrWhiteSpace(subClaim.Value))
                throw new UnauthorizedAccessException("User ID claim not found in token");

            if (Guid.TryParse(subClaim.Value, out var userId))
                return userId;

            throw new InvalidOperationException(
                $"User ID claim is not a valid GUID: {subClaim.Value}");
        }

        /// <summary>
        /// Tries to get user ID, returns null if not found or invalid.
        /// </summary>
        public static Guid? TryGetUserId(this ClaimsPrincipal user)
        {
            var subClaim = user.FindFirst("sub")
                           ?? user.FindFirst(ClaimTypes.NameIdentifier);

            if (subClaim != null && Guid.TryParse(subClaim.Value, out var userId))
                return userId;

            return null;
        }

        /// <summary>
        /// Safely extracts user email from claims.
        /// </summary>
        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst("email")?.Value
                ?? user.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Returns preferred username or display name.
        /// </summary>
        public static string? GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst("preferred_username")?.Value
                ?? user.Identity?.Name;
        }

        /// <summary>
        /// Checks if user has a given role claim.
        /// </summary>
        public static bool IsInRole(this ClaimsPrincipal user, string roleName)
        {
            return user.HasClaim(ClaimTypes.Role, roleName) ||
                   user.HasClaim("role", roleName);
        }
    }
}