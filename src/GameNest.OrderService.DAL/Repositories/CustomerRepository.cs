using Dapper;
using GameNest.OrderService.BLL.DTOs.Customer;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using System.Data;

namespace GameNest.OrderService.DAL.Repositories
{
    public class CustomerRepository : GenericRepository<CustomerDto, CustomerCreateDto, CustomerUpdateDto>, ICustomerRepository
    {
        public CustomerRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base("customer", connection, transaction)
        {
        }

        public async Task<CustomerDto?> GetByEmailAsync(string email)
        {
            var query = "SELECT * FROM customer WHERE email = @Email AND is_deleted = FALSE";
            return await _connection.QuerySingleOrDefaultAsync<CustomerDto>(query, new { Email = email }, _transaction);
        }

        public async Task<CustomerDto?> GetByUsernameAsync(string username)
        {
            var query = "SELECT * FROM customer WHERE username = @Username AND is_deleted = FALSE";
            return await _connection.QuerySingleOrDefaultAsync<CustomerDto>(query, new { Username = username }, _transaction);
        }
    }
}
