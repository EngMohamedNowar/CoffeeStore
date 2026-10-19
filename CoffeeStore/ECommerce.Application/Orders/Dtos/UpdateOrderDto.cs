using ECommerce.Domain.Entities.Enums;

namespace ECommerce.Application.Orders.Dtos;

public class UpdateOrderDto
{
    public OrderStatus Status { get; set; }
}
