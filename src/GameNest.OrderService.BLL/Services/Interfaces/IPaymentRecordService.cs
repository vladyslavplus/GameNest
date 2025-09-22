using GameNest.OrderService.BLL.DTOs.PaymentRecord;

namespace GameNest.OrderService.BLL.Services.Interfaces
{
    public interface IPaymentRecordService
    {
        Task<PaymentRecordDto> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<PaymentRecordDto>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<PaymentRecordDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
        Task<PaymentRecordDto> CreateAsync(PaymentRecordCreateDto dto, CancellationToken ct = default);
        Task<PaymentRecordDto> UpdateAsync(Guid id, PaymentRecordUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default);
    }
}
