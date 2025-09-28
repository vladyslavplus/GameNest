using GameNest.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNest.CatalogService.DAL.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("game");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Price).HasPrecision(10, 2).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasIndex(x => new { x.Title }).IsUnique();

            builder.HasMany(x => x.GameDeveloperRoles)
                   .WithOne(x => x.Game)
                   .HasForeignKey(x => x.GameId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.GamePlatforms)
                   .WithOne(x => x.Game)
                   .HasForeignKey(x => x.GameId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.GameGenres)
                   .WithOne(x => x.Game)
                   .HasForeignKey(x => x.GameId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}