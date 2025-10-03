namespace GameNest.ReviewsService.Infrastructure.Mongo.Indexes
{
    public interface IIndexCreationService
    {
        Task CreateIndexesAsync(CancellationToken cancellationToken = default);
    }
}
