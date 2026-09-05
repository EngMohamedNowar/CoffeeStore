using CoffeeStore.Domain.Entities.Addresses;
using CoffeeStore.Domain.Entities.Orders;
using ECommerce.Domain.Common;
using System.Net;

namespace CoffeeStore.Domain.Entities.Customers;

public class Customer : BaseEntity
{
    public string IdentityUserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}