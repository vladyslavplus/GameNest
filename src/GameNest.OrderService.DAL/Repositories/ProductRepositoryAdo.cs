using GameNest.OrderService.BLL.DTOs.Product;
using GameNest.OrderService.DAL.Repositories.Interfaces;
using Npgsql;
using System.Data;

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

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = "SELECT id, title, description, price FROM products WHERE id = @Id AND is_deleted = FALSE";

            var param = cmd.CreateParameter();
            param.ParameterName = "@Id";
            param.Value = id;
            cmd.Parameters.Add(param);

            using var reader = await Task.Run(() => cmd.ExecuteReader());
            if (reader.Read())
            {
                return new ProductDto
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price"))
                };
            }

            return null;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var result = new List<ProductDto>();
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = "SELECT id, title, description, price FROM products WHERE is_deleted = FALSE";

            using var reader = await Task.Run(() => cmd.ExecuteReader());
            while (reader.Read())
            {
                result.Add(new ProductDto
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price"))
                });
            }

            return result;
        }

        public async Task<Guid> CreateAsync(ProductCreateDto dto)
        {
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = "INSERT INTO products (title, description, price) VALUES (@Title, @Description, @Price) RETURNING id";

            var p1 = cmd.CreateParameter();
            p1.ParameterName = "@Title";
            p1.Value = dto.Title;
            cmd.Parameters.Add(p1);

            var p2 = cmd.CreateParameter();
            p2.ParameterName = "@Description";
            p2.Value = dto.Description;
            cmd.Parameters.Add(p2);

            var p3 = cmd.CreateParameter();
            p3.ParameterName = "@Price";
            p3.Value = dto.Price;
            cmd.Parameters.Add(p3);

            return await Task.Run(() => (Guid)cmd.ExecuteScalar()!);
        }

        public async Task UpdateAsync(Guid id, ProductUpdateDto dto)
        {
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = "UPDATE products SET title=@Title, description=@Description, price=@Price WHERE id=@Id AND is_deleted = FALSE";

            cmd.Parameters.Add(new NpgsqlParameter("@Title", dto.Title));
            cmd.Parameters.Add(new NpgsqlParameter("@Description", dto.Description));
            cmd.Parameters.Add(new NpgsqlParameter("@Price", dto.Price));
            cmd.Parameters.Add(new NpgsqlParameter("@Id", id));

            var affected = await Task.Run(() => cmd.ExecuteNonQuery());
            if (affected == 0) throw new KeyNotFoundException($"Product with Id {id} not found");
        }

        public async Task DeleteAsync(Guid id)
        {
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = "UPDATE products SET is_deleted = TRUE WHERE id=@Id";

            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@Id", id));

            var affected = await Task.Run(() => cmd.ExecuteNonQuery());
            if (affected == 0) throw new KeyNotFoundException($"Product with Id {id} not found");
        }

        public async Task<IEnumerable<ProductDto>> SearchByTitleAsync(string titlePart)
        {
            var result = new List<ProductDto>();
            using var cmd = _connection.CreateCommand();
            cmd.Transaction = _transaction;
            cmd.CommandText = "SELECT id, title, description, price FROM products WHERE title ILIKE @Title AND is_deleted = FALSE";

            cmd.Parameters.Add(new NpgsqlParameter("@Title", $"%{titlePart}%"));

            using var reader = await Task.Run(() => cmd.ExecuteReader());
            while (reader.Read())
            {
                result.Add(new ProductDto
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price"))
                });
            }

            return result;
        }
    }
}