using Dapper;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using GameNest.OrderService.Domain.Entities;
using System.Data;

namespace GameNest.OrderService.DAL.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base("customer", connection, transaction)
        {
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            var query = "SELECT * FROM customer WHERE email = @Email AND is_deleted = FALSE";
            return await _connection.QuerySingleOrDefaultAsync<Customer>(query, new { Email = email }, _transaction);
        }

        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            var query = "SELECT * FROM customer WHERE username = @Username AND is_deleted = FALSE";
            return await _connection.QuerySingleOrDefaultAsync<Customer>(query, new { Username = username }, _transaction);
        }
    }
}
