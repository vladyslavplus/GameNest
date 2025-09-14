using Dapper;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using GameNest.OrderService.Domain.Entities;
using System.Data;

namespace GameNest.OrderService.DAL.Repositories
{
    public class PaymentRecordRepository : GenericRepository<PaymentRecord>, IPaymentRecordRepository
    {
        public PaymentRecordRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base("payment_record", connection, transaction)
        {
        }

        public async Task<IEnumerable<PaymentRecord>> GetByOrderIdAsync(Guid orderId)
        {
            var query = "SELECT * FROM payment_record WHERE order_id = @OrderId AND is_deleted = FALSE";
            return await _connection.QueryAsync<PaymentRecord>(query, new { OrderId = orderId }, _transaction);
        }
    }
}
