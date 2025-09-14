using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace GameNest.OrderService.DAL.Infrastructure
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private string? _connectionString;

        public ConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            string? connStr = !string.IsNullOrEmpty(_connectionString)
                ? _connectionString
                : _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connStr))
                throw new InvalidOperationException("Connection string is not configured.");

            var connection = new NpgsqlConnection(connStr);
            connection.Open();
            return connection;
        }
    }
}
