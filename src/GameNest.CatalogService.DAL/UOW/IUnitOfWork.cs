using GameNest.CatalogService.DAL.Repositories.Interfaces;

namespace GameNest.CatalogService.DAL.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IGameRepository Games { get; }
        IDeveloperRepository Developers { get; }
        IGenreRepository Genres { get; }
        IPlatformRepository Platforms { get; }
        IPublisherRepository Publishers { get; }
        IRoleRepository Roles { get; }
        IGameDeveloperRoleRepository GameDeveloperRoles { get; }
        IGameGenreRepository GameGenres { get; }
        IGamePlatformRepository GamePlatforms { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}