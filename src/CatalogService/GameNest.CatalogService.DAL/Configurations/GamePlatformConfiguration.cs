using GameNest.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNest.CatalogService.DAL.Configurations
{
    public class GamePlatformConfiguration : IEntityTypeConfiguration<GamePlatform>
    {
        public void Configure(EntityTypeBuilder<GamePlatform> builder)
        {
            builder.ToTable("game_platform");

            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.GameId, x.PlatformId }).IsUnique();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}
