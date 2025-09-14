using GameNest.OrderService.BLL.DTOs.Product;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface IProductRepository
        : IGenericRepository<ProductDto, ProductCreateDto, ProductUpdateDto>
    {
        Task<IEnumerable<ProductDto>> SearchByTitleAsync(string titlePart);
    }
}
