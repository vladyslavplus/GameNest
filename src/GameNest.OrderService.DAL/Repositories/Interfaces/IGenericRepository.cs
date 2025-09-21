using GameNest.OrderService.Domain.Entities;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity>
        where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default);
        Task<Guid> CreateAsync(TEntity entity, CancellationToken ct = default);
        Task UpdateAsync(TEntity entity, CancellationToken ct = default);
        Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default);
    }
}