using System.Data;

namespace GameNest.OrderService.DAL.Infrastructure
{
    public interface IConnectionFactory
    {
        void SetConnection(string connectionString);
        IDbConnection GetConnection();
    }
}
