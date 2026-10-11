using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.Orders.Dtos;

namespace ECommerce.Application.Services.Contracts;

public interface IOrderServices
{
    Task<Result<OrderToReturn>> CreateOrderAsync(OrderDto dto,string email, CancellationToken ct);
}
