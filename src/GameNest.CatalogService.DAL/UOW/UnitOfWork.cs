using GameNest.CatalogService.DAL.Data;
using GameNest.CatalogService.DAL.Repositories;
using GameNest.CatalogService.DAL.Repositories.Interfaces;

namespace GameNest.CatalogService.DAL.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CatalogDbContext _context;
        private bool _disposed = false;
        public IGameRepository Games { get; }
        public IDeveloperRepository Developers { get; }
        public IGenreRepository Genres { get; }
        public IPlatformRepository Platforms { get; }
        public IPublisherRepository Publishers { get; }
        public IRoleRepository Roles { get; }
        public IGameDeveloperRoleRepository GameDeveloperRoles { get; }
        public IGameGenreRepository GameGenres { get; }
        public IGamePlatformRepository GamePlatforms { get; }

        public UnitOfWork(CatalogDbContext context)
        {
            _context = context;

            Games = new GameRepository(_context);
            Developers = new DeveloperRepository(_context);
            Genres = new GenreRepository(_context);
            Platforms = new PlatformRepository(_context);
            Publishers = new PublisherRepository(_context);
            Roles = new RoleRepository(_context);
            GameDeveloperRoles = new GameDeveloperRoleRepository(_context);
            GameGenres = new GameGenreRepository(_context);
            GamePlatforms = new GamePlatformRepository(_context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
