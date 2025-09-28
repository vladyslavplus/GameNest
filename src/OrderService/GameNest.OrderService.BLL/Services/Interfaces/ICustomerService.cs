using GameNest.OrderService.BLL.DTOs.Customer;

namespace GameNest.OrderService.BLL.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken ct = default);
        Task<CustomerDto> CreateAsync(CustomerCreateDto dto, CancellationToken ct = default);
        Task<CustomerDto> UpdateAsync(Guid id, CustomerUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default);
    }
}