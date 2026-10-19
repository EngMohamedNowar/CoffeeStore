using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;

namespace ECommerce.Application.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid Id, string Email)
    : IRequest<Result<OrderResponse>>;
