namespace GameNest.CatalogService.BLL.Cache.Services
{
    public interface IGameCacheInvalidationService
    {
        Task InvalidateGameAsync(Guid gameId);
        Task InvalidateAllGamesAsync();
    }
}
