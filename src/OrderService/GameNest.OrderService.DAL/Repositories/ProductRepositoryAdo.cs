using GameNest.OrderService.DAL.Repositories.Interfaces;
using GameNest.OrderService.Domain.Entities;
using Npgsql;
using System.Data;
using System.Threading;

namespace GameNest.OrderService.DAL.Repositories
{
    public class ProductRepositoryAdo : IProductRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction? _transaction;

        public ProductRepositoryAdo(IDbConnection connection, IDbTransaction? transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = "SELECT * FROM product WHERE id = @Id AND is_deleted = FALSE";
            cmd.Parameters.Add(new NpgsqlParameter("@Id", id));

            using var reader = await Task.Run(() => cmd.ExecuteReader(), ct);
            return reader.Read() ? MapReaderToProduct(reader) : null;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default)
        {
            var result = new List<Product>();
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = "SELECT * FROM product WHERE is_deleted = FALSE";

            using var reader = await Task.Run(() => cmd.ExecuteReader(), ct);
            while (reader.Read())
                result.Add(MapReaderToProduct(reader));

            return result;
        }

        public async Task<Guid> CreateAsync(Product entity, CancellationToken ct = default)
        {
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = @"
                INSERT INTO product 
                    (title, description, price, created_at, updated_at, created_by, updated_by, is_deleted)
                VALUES 
                    (@Title, @Description, @Price, @Created_At, @Updated_At, @Created_By, @Updated_By, @Is_Deleted)
                RETURNING id";

            cmd.Parameters.Add(new NpgsqlParameter("@Title", entity.Title));
            cmd.Parameters.Add(new NpgsqlParameter("@Description", (object?)entity.Description ?? DBNull.Value));
            cmd.Parameters.Add(new NpgsqlParameter("@Price", entity.Price));
            cmd.Parameters.Add(new NpgsqlParameter("@Created_At", entity.Created_At));
            cmd.Parameters.Add(new NpgsqlParameter("@Updated_At", entity.Updated_At));
            cmd.Parameters.Add(new NpgsqlParameter("@Created_By", (object?)entity.Created_By ?? DBNull.Value));
            cmd.Parameters.Add(new NpgsqlParameter("@Updated_By", (object?)entity.Updated_By ?? DBNull.Value));
            cmd.Parameters.Add(new NpgsqlParameter("@Is_Deleted", entity.Is_Deleted));

            return await Task.Run(() => (Guid)cmd.ExecuteScalar()!, ct);
        }

        public async Task UpdateAsync(Product entity, CancellationToken ct = default)
        {
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = @"
                UPDATE product
                SET title=@Title, description=@Description, price=@Price, updated_at=@Updated_At, updated_by=@Updated_By
                WHERE id=@Id AND is_deleted = FALSE";

            cmd.Parameters.Add(new NpgsqlParameter("@Title", entity.Title));
            cmd.Parameters.Add(new NpgsqlParameter("@Description", (object?)entity.Description ?? DBNull.Value));
            cmd.Parameters.Add(new NpgsqlParameter("@Price", entity.Price));
            cmd.Parameters.Add(new NpgsqlParameter("@Updated_At", entity.Updated_At));
            cmd.Parameters.Add(new NpgsqlParameter("@Updated_By", (object?)entity.Updated_By ?? DBNull.Value));
            cmd.Parameters.Add(new NpgsqlParameter("@Id", entity.Id));

            var affected = await Task.Run(() => cmd.ExecuteNonQuery(), ct);
            if (affected == 0) throw new KeyNotFoundException($"Product with Id {entity.Id} not found");
        }

        public async Task DeleteAsync(Guid id, bool softDelete = true, CancellationToken ct = default)
        {
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;

            if (softDelete)
            {
                cmd.CommandText = "UPDATE product SET is_deleted = TRUE, updated_at=@UpdatedAt WHERE id=@Id";
                cmd.Parameters.Add(new NpgsqlParameter("@UpdatedAt", DateTime.UtcNow));
            }
            else
            {
                cmd.CommandText = "DELETE FROM product WHERE id=@Id";
            }

            cmd.Parameters.Add(new NpgsqlParameter("@Id", id));

            var affected = await Task.Run(() => cmd.ExecuteNonQuery(), ct);
            if (affected == 0)
                throw new KeyNotFoundException($"Product with Id {id} not found");
        }

        public async Task<IEnumerable<Product>> SearchByTitleAsync(string titlePart, CancellationToken ct = default)
        {
            var result = new List<Product>();
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = "SELECT * FROM product WHERE title ILIKE @Title AND is_deleted = FALSE";
            cmd.Parameters.Add(new NpgsqlParameter("@Title", $"%{titlePart}%"));

            using var reader = await Task.Run(() => cmd.ExecuteReader(), ct);
            while (reader.Read())
                result.Add(MapReaderToProduct(reader));

            return result;
        }

        private static Product MapReaderToProduct(IDataReader reader)
        {
            return new Product
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                Created_At = reader.GetDateTime(reader.GetOrdinal("created_at")),
                Updated_At = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                Created_By = reader.IsDBNull(reader.GetOrdinal("created_by")) ? null : reader.GetGuid(reader.GetOrdinal("created_by")),
                Updated_By = reader.IsDBNull(reader.GetOrdinal("updated_by")) ? null : reader.GetGuid(reader.GetOrdinal("updated_by")),
                Is_Deleted = reader.GetBoolean(reader.GetOrdinal("is_deleted"))
            };
        }
    }
}