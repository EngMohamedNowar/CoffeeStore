namespace ECommerce.Domain.Entities.Orders;

public sealed record ProductItemOrdered(
    Guid ProductVariantId,
    string ProductName,
    int WeightInGrams,
    decimal UnitPrice);
