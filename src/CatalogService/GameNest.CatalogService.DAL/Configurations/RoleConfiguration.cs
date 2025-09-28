using GameNest.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNest.CatalogService.DAL.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("role");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasIndex(x => new { x.Name }).IsUnique();

            builder.HasMany(x => x.GameDeveloperRoles)
               .WithOne(x => x.Role)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
