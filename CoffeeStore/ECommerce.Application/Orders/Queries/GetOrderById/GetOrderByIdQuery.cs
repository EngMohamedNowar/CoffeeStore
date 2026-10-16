using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;

namespace ECommerce.Application.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid Id) : IQuery<Result<OrderResponse>>;
