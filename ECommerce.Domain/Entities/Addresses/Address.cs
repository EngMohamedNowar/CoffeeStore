using CoffeeStore.Domain.Entities.Customers;
using ECommerce.Domain.Common;

namespace CoffeeStore.Domain.Entities.Addresses;

public class Address : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public string Label { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? Building { get; set; }
    public string? Notes { get; set; }
    public bool IsDefault { get; set; } = false;
}