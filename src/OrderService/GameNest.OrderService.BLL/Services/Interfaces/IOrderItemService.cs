using GameNest.OrderService.BLL.DTOs.OrderItem;

namespace GameNest.OrderService.BLL.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<OrderItemDto> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<OrderItemDto>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    }
}
