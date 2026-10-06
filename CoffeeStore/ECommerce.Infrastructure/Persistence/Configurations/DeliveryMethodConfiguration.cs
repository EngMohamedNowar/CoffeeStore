using ECommerce.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class DeliveryMethodConfiguration : IEntityTypeConfiguration<DeliveryMethod>
{
    public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
    {
        builder.ToTable("DeliveryMethods");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(DeliveryMethod.MaxNameLength);

        builder.HasIndex(d => d.Name)
            .IsUnique();

        builder.Property(d => d.Description)
            .HasMaxLength(DeliveryMethod.MaxDescriptionLength);

        builder.Property(d => d.Price)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(d => d.EstimatedDeliveryTime)
            .IsRequired()
            .HasMaxLength(DeliveryMethod.MaxDeliveryTimeLength);

        builder.HasIndex(d => d.DisplayOrder);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
