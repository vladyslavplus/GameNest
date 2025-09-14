using GameNest.OrderService.BLL.DTOs.PaymentRecord;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface IPaymentRecordRepository
        : IGenericRepository<PaymentRecordDto, PaymentRecordCreateDto, PaymentRecordUpdateDto>
    {
        Task<IEnumerable<PaymentRecordDto>> GetByOrderIdAsync(Guid orderId);
    }
}
