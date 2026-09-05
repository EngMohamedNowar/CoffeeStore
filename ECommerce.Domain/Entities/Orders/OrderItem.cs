using CoffeeStore.Domain.Entities.Products;
using ECommerce.Domain.Common;

namespace CoffeeStore.Domain.Entities.Orders;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;

    public string ProductNameSnapshot { get; set; } = string.Empty;
    public int WeightInGramsSnapshot { get; set; }
    public decimal UnitPriceSnapshot { get; set; }

    public int Quantity { get; set; }
    public decimal LineTotal => UnitPriceSnapshot * Quantity;
}