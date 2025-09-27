using System.Linq.Dynamic.Core;
using GameNest.CatalogService.DAL.Helpers;

namespace GameNest.CatalogService.DAL.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySorting<T>(
            this IQueryable<T> query,
            string? orderBy,
            ISortHelper<T>? sortHelper = null)
        {
            var orderByQuery = !string.IsNullOrEmpty(orderBy) ? orderBy : "Id";

            if (sortHelper != null)
                return sortHelper.ApplySort(query, orderByQuery);

            return query.OrderByDynamic(orderByQuery);
        }

        private static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string propertyName)
        {
            return query.OrderBy(propertyName); 
        }
    }
}
