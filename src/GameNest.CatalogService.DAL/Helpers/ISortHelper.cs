namespace GameNest.CatalogService.DAL.Helpers
{
    public interface ISortHelper<T>
    {
        IQueryable<T> ApplySort(IQueryable<T> entities, string? orderByQueryString);
    }
}
