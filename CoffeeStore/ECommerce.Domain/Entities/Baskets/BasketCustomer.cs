namespace ECommerce.Domain.Entities.Baskets;

public class BasketCustomer
{
    public Guid Id { get; set; }
    public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
}
