using MongoDB.Driver;

namespace GameNest.ReviewsService.Domain.Common.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public string? NextCursor { get; private set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize, string? nextCursor = null)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            NextCursor = nextCursor;
            AddRange(items);
        }

        public static async Task<PagedList<T>> ToPagedListAsync(
            IFindFluent<T, T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var count = await source.CountDocumentsAsync(cancellationToken: cancellationToken);
            var items = await source.Skip((pageNumber - 1) * pageSize)
                                    .Limit(pageSize)
                                    .ToListAsync(cancellationToken);

            return new PagedList<T>(items, (int)count, pageNumber, pageSize);
        }

        public static PagedList<T> FromCursor(List<T> items, int pageSize)
        {
            string? nextCursor = null;

            if (items.Count == pageSize && items[items.Count - 1] is BaseEntity lastEntity)
                nextCursor = lastEntity.Id;

            return new PagedList<T>(items, items.Count, 1, pageSize, nextCursor);
        }
    }
}