using GameNest.OrderService.Domain.Entities;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<Customer?> GetByUsernameAsync(string username, CancellationToken ct = default);
    }
}
