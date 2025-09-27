using GameNest.CatalogService.DAL.Helpers;
using GameNest.CatalogService.Domain.Entities.Parameters;

namespace GameNest.CatalogService.DAL.Extensions
{
    public static class QueryablePagingExtensions
    {
        public static Task<PagedList<T>> ToPagedListAsync<T, TParams>(
            this IQueryable<T> query,
            TParams parameters,
            CancellationToken cancellationToken = default)
            where TParams : QueryStringParameters
        {
            return PagedList<T>.ToPagedListAsync(
                query,
                parameters.PageNumber,
                parameters.PageSize,
                cancellationToken
            );
        }
    }
}
