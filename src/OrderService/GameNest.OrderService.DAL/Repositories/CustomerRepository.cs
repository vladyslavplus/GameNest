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

        public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var query = "SELECT * FROM customer WHERE email = @Email AND is_deleted = FALSE";
            return await _connection.QuerySingleOrDefaultAsync<Customer>(new CommandDefinition(
                query,
                new { Email = email },
                transaction: _transaction,
                cancellationToken: ct
            ));
        }

        public async Task<Customer?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            var query = "SELECT * FROM customer WHERE username = @Username AND is_deleted = FALSE";
            return await _connection.QuerySingleOrDefaultAsync<Customer>(new CommandDefinition(
                query,
                new { Username = username },
                transaction: _transaction,
                cancellationToken: ct
            ));
        }
    }
}
