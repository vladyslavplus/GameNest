using GameNest.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNest.CatalogService.DAL.Configurations
{
    public class DeveloperConfiguration : IEntityTypeConfiguration<Developer>
    {
        public void Configure(EntityTypeBuilder<Developer> builder)
        {
            builder.ToTable("developer");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(80);
            builder.Property(x => x.Country).HasMaxLength(80);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasMany(x => x.GameDeveloperRoles)
                   .WithOne(x => x.Developer)
                   .HasForeignKey(x => x.DeveloperId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}