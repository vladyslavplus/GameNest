using GameNest.IdentityService.DAL.Data;
using GameNest.IdentityService.DAL.Interfaces;
using GameNest.IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNest.IdentityService.DAL.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentityDbContext _context;

        public RefreshTokenRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
        }

        public async Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .Where(r => r.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            await _context.RefreshTokens.AddAsync(token, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
