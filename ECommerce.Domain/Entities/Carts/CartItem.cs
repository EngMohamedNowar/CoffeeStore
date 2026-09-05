using CoffeeStore.Domain.Entities.Products;
using ECommerce.Domain.Common;

namespace CoffeeStore.Domain.Entities.Carts;

public class CartItem : BaseEntity
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public Guid ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}