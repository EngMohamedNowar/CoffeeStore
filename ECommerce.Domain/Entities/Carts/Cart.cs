using CoffeeStore.Domain.Entities.Customers;
using ECommerce.Domain.Common;

namespace CoffeeStore.Domain.Entities.Carts;

public class Cart : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    public decimal TotalPrice => Items.Sum(i => i.UnitPrice * i.Quantity);
}