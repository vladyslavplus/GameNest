using GameNest.OrderService.Domain.Entities;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
        Task<IEnumerable<Order>> GetAllWithItemsAsync(CancellationToken ct = default);
        Task<Order?> GetWithItemsByIdAsync(Guid orderId, CancellationToken ct = default);
    }
}
