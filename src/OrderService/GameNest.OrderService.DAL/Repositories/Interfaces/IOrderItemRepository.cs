using GameNest.OrderService.Domain.Entities;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    }
}
