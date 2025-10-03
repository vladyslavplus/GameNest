using MongoDB.Driver;

namespace GameNest.ReviewsService.Infrastructure.Mongo.UOW
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        private readonly IMongoDatabase _database;
        private IClientSessionHandle? _session;
        private bool _disposed;

        public IClientSessionHandle Session => _session ?? throw new InvalidOperationException("Session has not been started.");

        public MongoUnitOfWork(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task StartTransactionAsync()
        {
            _session = await _database.Client.StartSessionAsync();
            _session.StartTransaction();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_session == null)
                throw new InvalidOperationException("No active session to commit.");

            await _session.CommitTransactionAsync(cancellationToken);
            DisposeSession();
        }

        public async Task AbortAsync(CancellationToken cancellationToken = default)
        {
            if (_session == null)
                return;

            await _session.AbortTransactionAsync(cancellationToken);
            DisposeSession();
        }

        private void DisposeSession()
        {
            _session?.Dispose();
            _session = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DisposeSession();
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