using GameNest.OrderService.BLL.DTOs.Customer;

namespace GameNest.OrderService.DAL.Repositories.Interfaces
{
    public interface ICustomerRepository
        : IGenericRepository<CustomerDto, CustomerCreateDto, CustomerUpdateDto>
    {
        Task<CustomerDto?> GetByEmailAsync(string email);
        Task<CustomerDto?> GetByUsernameAsync(string username);
    }
}
