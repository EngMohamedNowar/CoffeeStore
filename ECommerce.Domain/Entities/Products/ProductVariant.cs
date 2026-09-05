
using CoffeeStore.Domain.Entities.Enums;
using ECommerce.Domain.Common;

namespace CoffeeStore.Domain.Entities.Products;

public class ProductVariant : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int WeightInGrams { get; set; }
    public GrindType GrindType { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Sku { get; set; } = string.Empty;

    public bool IsInStock => StockQuantity > 0;
}