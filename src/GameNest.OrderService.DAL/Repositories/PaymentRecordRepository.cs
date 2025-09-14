using Dapper;
using GameNest.OrderService.BLL.DTOs.PaymentRecord;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using System.Data;

namespace GameNest.OrderService.DAL.Repositories
{
    public class PaymentRecordRepository : GenericRepository<PaymentRecordDto, PaymentRecordCreateDto, PaymentRecordUpdateDto>, IPaymentRecordRepository
    {
        public PaymentRecordRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base("payment_record", connection, transaction)
        {
        }

        public async Task<IEnumerable<PaymentRecordDto>> GetByOrderIdAsync(Guid orderId)
        {
            var query = "SELECT * FROM payment_records WHERE order_id = @OrderId AND is_deleted = FALSE";
            return await _connection.QueryAsync<PaymentRecordDto>(query, new { OrderId = orderId }, _transaction);
        }
    }
}
