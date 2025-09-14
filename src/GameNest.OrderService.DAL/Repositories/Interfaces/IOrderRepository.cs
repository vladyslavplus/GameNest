using GameNest.OrderService.BLL.DTOs.Order;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface IOrderRepository
        : IGenericRepository<OrderDto, OrderCreateDto, OrderUpdateDto>
    {
        Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(Guid customerId);
    }
}
