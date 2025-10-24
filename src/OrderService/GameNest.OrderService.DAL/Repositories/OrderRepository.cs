using Dapper;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using GameNest.OrderService.Domain.Entities;
using System.Data;

namespace GameNest.OrderService.DAL.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base("\"order\"", connection, transaction)
        {
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        {
            var query = "SELECT * FROM \"order\" WHERE customer_id = @CustomerId AND is_deleted = FALSE";

            return await _connection.QueryAsync<Order>(new CommandDefinition(
                query,
                new { CustomerId = customerId },
                transaction: _transaction,
                cancellationToken: ct
            ));
        }

        public async Task<IEnumerable<Order>> GetAllWithItemsAsync(CancellationToken ct = default)
        {
            var query = @"
                SELECT o.*, i.*
                FROM ""order"" o
                LEFT JOIN order_item i ON o.id = i.order_id
                WHERE o.is_deleted = FALSE
                ORDER BY o.created_at DESC";

            var orderDict = new Dictionary<Guid, Order>();
            await _connection.QueryAsync<Order, OrderItem, Order>(
                new CommandDefinition(query, transaction: _transaction, cancellationToken: ct),
                (order, item) =>
                {
                    if (!orderDict.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Items = new List<OrderItem>();
                        orderDict.Add(order.Id, currentOrder);
                    }

                    if (item != null)
                        currentOrder.Items.Add(item);

                    return currentOrder;
                },
                splitOn: "Id"
            );

            return orderDict.Values;
        }

        public async Task<Order?> GetWithItemsByIdAsync(Guid orderId, CancellationToken ct = default)
        {
            var query = @"
                SELECT o.*, i.*
                FROM ""order"" o
                LEFT JOIN order_item i ON o.id = i.order_id
                WHERE o.id = @OrderId AND o.is_deleted = FALSE";

            var orderDict = new Dictionary<Guid, Order>();
            var result = await _connection.QueryAsync<Order, OrderItem, Order>(
                new CommandDefinition(query, new { OrderId = orderId }, _transaction, cancellationToken: ct),
                (order, item) =>
                {
                    if (!orderDict.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Items = new List<OrderItem>();
                        orderDict.Add(order.Id, currentOrder);
                    }
                    if (item != null)
                        currentOrder.Items.Add(item);
                    return currentOrder;
                },
                splitOn: "Id"
            );
            return result.FirstOrDefault();
        }

        public override async Task<Guid> CreateAsync(Order order, CancellationToken ct = default)
        {
            var query = @"
                INSERT INTO ""order"" (
                    customer_id, status, total_amount,
                    country, city, street, zip_code
                )
                VALUES (
                    @CustomerId, @Status, @TotalAmount,
                    @Country, @City, @Street, @ZipCode
                )
                RETURNING id";

            return await _connection.ExecuteScalarAsync<Guid>(
                new CommandDefinition(query, new
                {
                    CustomerId = order.Customer_Id,
                    Status = order.Status,
                    TotalAmount = order.Total_Amount,
                    Country = order.Country,
                    City = order.City,
                    Street = order.Street,
                    ZipCode = order.ZipCode
                }, _transaction, cancellationToken: ct)
            );
        }

        public override async Task UpdateAsync(Order entity, CancellationToken ct = default)
        {
            var query = @"
                UPDATE ""order""
                SET status = @Status,
                    updated_at = @UpdatedAt
                WHERE id = @Id AND is_deleted = FALSE";

            var affected = await _connection.ExecuteAsync(
                new CommandDefinition(query, new
                {
                    Id = entity.Id,
                    Status = entity.Status,
                    UpdatedAt = DateTime.UtcNow
                }, _transaction, cancellationToken: ct)
            );

            if (affected == 0)
                throw new KeyNotFoundException($"Order with Id {entity.Id} not found");
        }
    }
}