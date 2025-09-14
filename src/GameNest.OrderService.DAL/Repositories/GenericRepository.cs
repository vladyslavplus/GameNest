using Dapper;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using GameNest.OrderService.Domain.Entities;
using System.Data;
using System.Reflection;

namespace GameNest.OrderService.DAL.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly IDbConnection _connection;
        protected readonly IDbTransaction? _transaction;
        private readonly string _tableName;
        private readonly bool _softDelete;

        public GenericRepository(string tableName, IDbConnection connection, IDbTransaction? transaction = null, bool softDelete = true)
        {
            _tableName = tableName;
            _connection = connection;
            _transaction = transaction;
            _softDelete = softDelete;
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            var query = $"SELECT * FROM {_tableName} WHERE id = @Id";
            if (_softDelete)
                query += " AND is_deleted = FALSE";

            return await _connection.QuerySingleOrDefaultAsync<TEntity>(query, new { Id = id }, _transaction);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var query = $"SELECT * FROM {_tableName}";
            if (_softDelete)
                query += " WHERE is_deleted = FALSE";

            return await _connection.QueryAsync<TEntity>(query, transaction: _transaction);
        }

        public virtual async Task<Guid> CreateAsync(TEntity entity)
        {
            var columns = GetColumns<TEntity>();
            var columnsString = string.Join(", ", columns);
            var paramsString = string.Join(", ", columns.Select(c => "@" + c));

            var query = $"INSERT INTO {_tableName} ({columnsString}) VALUES ({paramsString}) RETURNING id";

            return await _connection.ExecuteScalarAsync<Guid>(query, entity, _transaction);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            var columns = GetColumns<TEntity>();
            var setString = string.Join(", ", columns.Select(c => $"{c} = @{c}"));

            var query = $"UPDATE {_tableName} SET {setString} WHERE id = @Id";
            if (_softDelete)
                query += " AND is_deleted = FALSE";

            var affected = await _connection.ExecuteAsync(query, entity, _transaction);
            if (affected == 0)
                throw new KeyNotFoundException($"{_tableName} with Id {entity.Id} not found");
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var query = _softDelete
                ? $"UPDATE {_tableName} SET is_deleted = TRUE WHERE id = @Id"
                : $"DELETE FROM {_tableName} WHERE id = @Id";

            var affected = await _connection.ExecuteAsync(query, new { Id = id }, _transaction);
            if (affected == 0)
                throw new KeyNotFoundException($"{_tableName} with Id {id} not found");
        }

        private static IEnumerable<string> GetColumns<T>()
        {
            var auditFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id", "Created_At", "Created_By", "Updated_At", "Updated_By", "Is_Deleted"
            };

            return typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !auditFields.Contains(p.Name))
                .Select(p => p.Name);
        }
    }
}
