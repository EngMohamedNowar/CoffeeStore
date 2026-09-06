using CoffeeStore.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeStore.Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.GrindType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(v => v.Price)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(v => v.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(v => v.Sku)
            .IsUnique();

        builder.Ignore(v => v.IsInStock); // computed property, not mapped

        builder.HasQueryFilter(v => !v.IsDeleted);
    }
}