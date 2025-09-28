using GameNest.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNest.CatalogService.DAL.Configurations
{
    public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {
            builder.ToTable("publisher");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Type).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Country).HasMaxLength(100);
            builder.Property(x => x.Phone).HasMaxLength(60);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasIndex(x => new { x.Name }).IsUnique();

            builder.HasMany(x => x.Games)
               .WithOne(x => x.Publisher)
               .HasForeignKey(x => x.PublisherId)
               .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
