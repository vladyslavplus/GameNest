using GameNest.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNest.CatalogService.DAL.Data
{
    public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
    {
        public DbSet<Game> Games { get; set; } 
        public DbSet<Developer> Developers { get; set; } 
        public DbSet<Publisher> Publishers { get; set; } 
        public DbSet<Genre> Genres { get; set; } 
        public DbSet<Platform> Platforms { get; set; } 
        public DbSet<Role> Roles { get; set; }
        public DbSet<GameDeveloperRole> GameDeveloperRoles { get; set; } 
        public DbSet<GameGenre> GameGenres { get; set; } 
        public DbSet<GamePlatform> GamePlatforms { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        }
    }
}
