using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DeliveryMethods.Dtos;

public sealed record DeliveryMethodResponse(Guid Id,
    string Name, string? Description, decimal Price,
    string EstimatedDeliveryTime, bool IsAvailable, int DiplayOrder)
{
}
