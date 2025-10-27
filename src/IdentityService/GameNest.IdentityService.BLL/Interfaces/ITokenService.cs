using GameNest.IdentityService.BLL.DTOs;
using GameNest.IdentityService.Domain.Entities;

namespace GameNest.IdentityService.BLL.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponseDto> GenerateTokensAsync(ApplicationUser user, CancellationToken cancellationToken = default);
        Task<AuthResponseDto> RefreshTokensAsync(TokenRequestDto tokenRequestDto, CancellationToken cancellationToken = default);
        Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
