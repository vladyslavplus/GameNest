using GameNest.OrderService.Domain.Entities;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> SearchByTitleAsync(string titlePart, CancellationToken ct = default);
    }
}
