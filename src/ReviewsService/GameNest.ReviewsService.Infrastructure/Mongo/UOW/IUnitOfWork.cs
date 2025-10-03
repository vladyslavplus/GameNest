using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Mongo.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IClientSessionHandle Session { get; }
        Task StartTransactionAsync();
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task AbortAsync(CancellationToken cancellationToken = default);
    }
}
