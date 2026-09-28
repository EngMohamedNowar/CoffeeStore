using ECommerce.Domain.Entities.Addresses;
using ECommerce.Domain.Entities.Customers;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities.Orders;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid ShippingAddressId { get; set; }
    public Address ShippingAddress { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public decimal SubTotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public Payment? Payment { get; set; }
}