using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.Orders.Dtos;

namespace ECommerce.Application.DTOs.Orders;

public class OrderDto
{
    public Guid BasketId { get; set; }
    public Guid DeliveryMethodId { get; set; }
    public OrderAddressToReturn ShipToAddress { get; set; }
}
