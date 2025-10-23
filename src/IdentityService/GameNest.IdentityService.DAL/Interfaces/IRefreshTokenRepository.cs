using GameNest.IdentityService.Domain.Entities;

namespace GameNest.IdentityService.DAL.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
