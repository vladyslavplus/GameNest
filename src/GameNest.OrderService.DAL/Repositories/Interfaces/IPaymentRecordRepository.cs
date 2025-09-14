using GameNest.OrderService.Domain.Entities;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface IPaymentRecordRepository : IGenericRepository<PaymentRecord>
    {
        Task<IEnumerable<PaymentRecord>> GetByOrderIdAsync(Guid orderId);
    }
}
