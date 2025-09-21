using Dapper;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using GameNest.OrderService.Domain.Entities;
using System.Data;

namespace GameNest.OrderService.DAL.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base("order_item", connection, transaction)
        {
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            var query = "SELECT * FROM order_item WHERE order_id = @OrderId AND is_deleted = FALSE";
            return await _connection.QueryAsync<OrderItem>(new CommandDefinition(
                query,
                new { OrderId = orderId },
                transaction: _transaction,
                cancellationToken: ct
            ));
        }
    }
}
