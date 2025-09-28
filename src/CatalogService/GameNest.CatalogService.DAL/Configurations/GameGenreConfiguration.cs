using GameNest.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNest.CatalogService.DAL.Configurations
{
    public class GameGenreConfiguration : IEntityTypeConfiguration<GameGenre>
    {
        public void Configure(EntityTypeBuilder<GameGenre> builder)
        {
            builder.ToTable("game_genre");

            builder.HasKey(x => x.Id); 
            builder.HasIndex(x => new { x.GameId, x.GenreId }).IsUnique(); 

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}
