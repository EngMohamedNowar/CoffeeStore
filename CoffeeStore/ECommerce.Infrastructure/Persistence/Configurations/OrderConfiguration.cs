using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.SubTotal)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(o => o.ShippingFee)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(o => o.UserEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.OwnsOne(o => o.ShipToAddress, shipTo =>
        {
            shipTo.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            shipTo.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(50);

            shipTo.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200);

            shipTo.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            shipTo.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100);

            shipTo.WithOwner();
        });

        builder.HasOne(o => o.DeliveryMethod)
            .WithMany()
            .HasForeignKey(o => o.DeliveryMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Payment)
            .WithOne(p => p.Order)
            .HasForeignKey<Payment>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.Status);

        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}