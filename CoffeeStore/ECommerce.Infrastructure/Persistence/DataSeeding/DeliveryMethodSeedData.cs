using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Infrastructure.Persistence.DataSeeding;

public sealed class DeliveryMethodSeedData
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public string EstimatedDeliveryTime { get; init; } = string.Empty;

    public bool IsAvailable { get; init; } = true;

    public int DisplayOrder { get; init; }

    public Result<DeliveryMethod> ToDomain()
        => DeliveryMethod.Create(
            Id,
            Name,
            Price,
            EstimatedDeliveryTime,
            Description,
            IsAvailable,
            DisplayOrder);
}
