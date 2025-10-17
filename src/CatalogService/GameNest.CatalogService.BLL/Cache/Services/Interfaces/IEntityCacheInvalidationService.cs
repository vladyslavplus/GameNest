namespace GameNest.CatalogService.BLL.Cache.Services.Interfaces
{
    public interface IEntityCacheInvalidationService<T>
    {
        Task InvalidateByIdAsync(Guid entityId);
        Task InvalidateAllAsync();
        Type EntityType => typeof(T);
    }
}
