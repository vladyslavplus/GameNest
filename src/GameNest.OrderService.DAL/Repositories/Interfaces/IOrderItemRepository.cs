using GameNest.OrderService.BLL.DTOs.OrderItem;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface IOrderItemRepository
        : IGenericRepository<OrderItemDto, OrderItemCreateDto, OrderItemUpdateDto>
    {
        Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(Guid orderId);
    }
}
