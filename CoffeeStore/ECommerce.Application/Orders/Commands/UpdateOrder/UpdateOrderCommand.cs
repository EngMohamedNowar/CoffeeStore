using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Domain.Entities.Enums;

namespace ECommerce.Application.Orders.Commands.UpdateOrder;

public sealed record UpdateOrderCommand(Guid Id, OrderStatus Status)
    : IRequest<Result<OrderResponse>>;
