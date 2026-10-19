using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;

namespace ECommerce.Application.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid Id, string Email, bool IsAdmin = false)
    : IQuery<Result<OrderResponse>>;
