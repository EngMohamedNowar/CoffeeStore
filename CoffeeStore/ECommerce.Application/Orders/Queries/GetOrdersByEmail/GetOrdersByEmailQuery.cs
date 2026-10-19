using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;

namespace ECommerce.Application.Orders.Queries.GetOrdersByEmail;

public sealed record GetOrdersByEmailQuery(string Email)
    : IQuery<Result<IReadOnlyList<OrderResponse>>>;
