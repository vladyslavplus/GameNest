using Dapper;
using GameNest.OrderService.BLL.DTOs.Order;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using System.Data;

namespace GameNest.OrderService.DAL.Repositories
{
    public class OrderRepository : GenericRepository<OrderDto, OrderCreateDto, OrderUpdateDto>, IOrderRepository
    {
        public OrderRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base("orders", connection, transaction)
        {
        }

        public async Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(Guid customerId)
        {
            var query = "SELECT * FROM orders WHERE customer_id = @CustomerId AND is_deleted = FALSE";
            return await _connection.QueryAsync<OrderDto>(query, new { CustomerId = customerId }, _transaction);
        }
    }
}
