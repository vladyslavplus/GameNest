using Microsoft.AspNetCore.Identity;

namespace GameNest.IdentityService.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
