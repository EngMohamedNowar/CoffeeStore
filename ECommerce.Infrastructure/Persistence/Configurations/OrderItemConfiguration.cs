using CoffeeStore.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeStore.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductNameSnapshot)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.UnitPriceSnapshot)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.HasOne(i => i.ProductVariant)
            .WithMany()
            .HasForeignKey(i => i.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(i => i.LineTotal); // computed property, not mapped

        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}