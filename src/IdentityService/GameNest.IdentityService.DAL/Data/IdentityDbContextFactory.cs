using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GameNest.IdentityService.DAL.Data
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

            var connectionString = "Host=localhost;Port=5432;Database=gamenest-identityservice-db;Username=postgres;Password=12345678";

            optionsBuilder.UseNpgsql(connectionString);

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}
