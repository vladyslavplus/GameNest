using Dapper;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using GameNest.OrderService.Domain.Entities;
using System.Data;

namespace GameNest.OrderService.DAL.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base("orders", connection, transaction)
        {
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId)
        {
            var query = "SELECT * FROM orders WHERE customer_id = @CustomerId AND is_deleted = FALSE";
            return await _connection.QueryAsync<Order>(query, new { CustomerId = customerId }, _transaction);
        }
    }
}
