using GameNest.OrderService.BLL.DTOs.Product;

namespace GameNest.OrderService.BLL.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default);
        Task<ProductDto> CreateAsync(ProductCreateDto dto, CancellationToken ct = default);
        Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default);
    }
}