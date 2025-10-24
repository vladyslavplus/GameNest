using GameNest.OrderService.DAL.Repositories.Interfaces;

namespace GameNest.OrderService.DAL.UOW
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IOrderRepository? Orders { get; }
        IOrderItemRepository? OrderItems { get; }
        IPaymentRecordRepository? PaymentRecords { get; }
        void BeginTransaction();
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
    }
}
