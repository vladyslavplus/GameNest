using GameNest.OrderService.DAL.Infrastructure;
using GameNest.OrderService.DAL.Repositories;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using System.Data;

namespace GameNest.OrderService.DAL.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IConnectionFactory _connectionFactory;
        private IDbConnection? _connection;
        private IDbTransaction? _transaction;
        private bool _disposed = false;
        private readonly object _lockObject = new object();

        public ICustomerRepository? Customers { get; private set; }
        public IProductRepository? Products { get; private set; }
        public IOrderRepository? Orders { get; private set; }
        public IOrderItemRepository? OrderItems { get; private set; }
        public IPaymentRecordRepository? PaymentRecords { get; private set; }
        public IConnectionFactory ConnectionFactory => _connectionFactory;

        public UnitOfWork(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.GetConnection();
            InitializeRepositories();
        }

        private void InitializeRepositories()
        {
            if (_connection == null) throw new InvalidOperationException("Connection is null");

            Customers = new CustomerRepository(_connection, _transaction);
            Products = new ProductRepositoryAdo(_connection, _transaction);
            Orders = new OrderRepository(_connection, _transaction);
            OrderItems = new OrderItemRepository(_connection, _transaction);
            PaymentRecords = new PaymentRecordRepository(_connection, _transaction);
        }

        private void EnsureTransactionStarted()
        {
            lock (_lockObject)
            {
                if (_transaction == null)
                {
                    if (_connection?.State != ConnectionState.Open)
                        _connection?.Open();

                    _transaction = _connection!.BeginTransaction();
                    InitializeRepositories();
                }
            }
        }

        public void BeginTransaction()
        {
            lock (_lockObject)
            {
                if (_transaction != null)
                    throw new InvalidOperationException("Transaction already started");

                EnsureTransactionStarted();
            }
        }

        public Task CommitAsync(CancellationToken ct = default)
        {
            lock (_lockObject)
            {
                EnsureTransactionStarted();

                if (_transaction == null)
                    throw new InvalidOperationException("No active transaction");

                try
                {
                    _transaction.Commit();
                }
                catch
                {
                    _transaction.Rollback();
                    throw;
                }
                finally
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }

            return Task.CompletedTask;
        }

        public Task RollbackAsync(CancellationToken ct = default)
        {
            lock (_lockObject)
            {
                if (_transaction == null)
                    return Task.CompletedTask; 

                try
                {
                    _transaction.Rollback();
                }
                finally
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_transaction != null)
            {
                await RollbackAsync();
            }

            if (_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();
                _connection.Dispose();
                _connection = null;
            }

            _disposed = true;
        }
    }
}