using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Ignore(i => i.LineTotal);

        builder.OwnsOne(i => i.ItemOrdered, itemOrdered =>
        {
            itemOrdered.Property(p => p.ProductVariantId)
                .HasColumnName("ProductVariantId");

            itemOrdered.Property(p => p.ProductName)
                .HasColumnName("ProductNameSnapshot")
                .IsRequired()
                .HasMaxLength(150);

            itemOrdered.Property(p => p.WeightInGrams)
                .HasColumnName("WeightInGramsSnapshot");

            itemOrdered.Property(p => p.UnitPrice)
                .HasColumnName("UnitPriceSnapshot")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            itemOrdered.WithOwner();

            itemOrdered.HasOne<ProductVariant>()
                .WithMany()
                .HasForeignKey("ProductVariantId")
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
