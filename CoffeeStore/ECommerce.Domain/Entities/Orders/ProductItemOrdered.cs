using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities.Orders;

public sealed record ProductItemOrdered
{
    private ProductItemOrdered()
    {
    }

    public Guid ProductVariantId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int WeightInGrams { get; private set; }
    public decimal UnitPrice { get; private set; }

    public static Result<ProductItemOrdered> Create(
        Guid productVariantId,
        string productName,
        int weightInGrams,
        decimal unitPrice)
    {
        if (productVariantId == Guid.Empty)
            return Result<ProductItemOrdered>.Failure(OrderErrors.InvalidProductId);

        if (string.IsNullOrWhiteSpace(productName))
            return Result<ProductItemOrdered>.Failure(OrderErrors.InvalidProductName);

        if (weightInGrams < 1)
            return Result<ProductItemOrdered>.Failure(OrderErrors.InvalidWeight);

        if (unitPrice < 0)
            return Result<ProductItemOrdered>.Failure(OrderErrors.InvalidUnitPrice);

        return Result<ProductItemOrdered>.Success(new ProductItemOrdered
        {
            ProductVariantId = productVariantId,
            ProductName = productName.Trim(),
            WeightInGrams = weightInGrams,
            UnitPrice = unitPrice
        });
    }
}
