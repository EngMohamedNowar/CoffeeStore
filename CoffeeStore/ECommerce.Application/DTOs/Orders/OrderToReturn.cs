using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Orders.Dtos;

public sealed class OrderToReturn
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    public Guid CustomerId { get; init; }
    public string UserEmail { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public OrderAddressToReturn ShipToAddress { get; init; } = null!;

    public Guid DeliveryMethodId { get; init; }
    public decimal DeliveryMethodPrice { get; init; }
    public decimal ShippingFee { get; init; }

    public IReadOnlyList<OrderItemToReturn> Items { get; init; } = Array.Empty<OrderItemToReturn>();

    public decimal SubTotal { get; init; }
    public decimal Total { get; init; }

    public static OrderToReturn FromOrder(Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        CreatedAt = order.CreatedAt,
        CustomerId = order.CustomerId,
        UserEmail = order.UserEmail,
        Status = order.Status.ToString(),
        ShipToAddress = new OrderAddressToReturn
        {
            FirstName = order.ShipToAddress.FirstName,
            LastName = order.ShipToAddress.LastName,
            Street = order.ShipToAddress.Street,
            City = order.ShipToAddress.City,
            Country = order.ShipToAddress.Country
        },
        DeliveryMethodId = order.DeliveryMethodId,
        DeliveryMethodPrice = order.DeliveryMethodPrice,
        ShippingFee = order.ShippingFee,
        Items = order.Items
            .Select(item => new OrderItemToReturn
            {
                Id = item.Id,
                Quantity = item.Quantity,
                UnitPrice = item.ItemOrdered.UnitPrice,
                LineTotal = item.LineTotal
            })
            .ToList(),
        SubTotal = order.SubTotal,
        Total = order.GetTotal()
    };
}

public sealed class OrderAddressToReturn
{
    [Required]
    public string FirstName { get; init; } = string.Empty;
    [Required]

    public string LastName { get; init; } = string.Empty;
    [Required]

    public string Street { get; init; } = string.Empty;
    [Required]

    public string City { get; init; } = string.Empty;
    [Required]

    public string Country { get; init; } = string.Empty;
}

public sealed class OrderItemToReturn
{
    public Guid Id { get; init; }
    [Required]

    public int Quantity { get; init; }
    [Required]

    public decimal UnitPrice { get; init; }
    [Required]
    public decimal LineTotal { get; init; }
}
