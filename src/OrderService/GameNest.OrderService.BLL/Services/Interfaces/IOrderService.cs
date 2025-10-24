using GameNest.OrderService.BLL.DTOs.Order;

namespace GameNest.OrderService.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct = default);
        Task<OrderDto> CreateAsync(Guid userId, OrderCreateDto dto, CancellationToken ct = default);
        Task<OrderDto> UpdateAsync(Guid id, OrderUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default);
    }
}
