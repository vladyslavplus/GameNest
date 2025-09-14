using Dapper;
using GameNest.OrderService.BLL.DTOs.OrderItem;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using System.Data;

namespace GameNest.OrderService.DAL.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItemDto, OrderItemCreateDto, OrderItemUpdateDto>, IOrderItemRepository
    {
        public OrderItemRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base("order_item", connection, transaction)
        {
        }

        public async Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(Guid orderId)
        {
            var query = "SELECT * FROM order_item WHERE order_id = @OrderId AND is_deleted = FALSE";
            return await _connection.QueryAsync<OrderItemDto>(query, new { OrderId = orderId }, _transaction);
        }
    }
}
