using GameNest.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNest.CatalogService.DAL.Configurations
{
    public class GameDeveloperRoleConfiguration : IEntityTypeConfiguration<GameDeveloperRole>
    {
        public void Configure(EntityTypeBuilder<GameDeveloperRole> builder)
        {
            builder.ToTable("game_developer_role");

            builder.HasKey(x => x.Id); 
            builder.HasIndex(x => new { x.GameId, x.DeveloperId, x.RoleId }).IsUnique(); 

            builder.Property(x => x.Seniority).HasMaxLength(20).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}
