using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Orders.Specification;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Errors;

namespace ECommerce.Application.Orders.Queries.GetOrderById;

internal class GetOrderByIdQueryHandler(IReadRepository<Order> repository)
    : IQueryHandler<GetOrderByIdQuery, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> HandleAsync(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var order = await repository.GetBySpecAsync(
            new OrderByIdSpecification(query.Id),
            cancellationToken);

        if (order is null)
            return Result<OrderResponse>.Fail(OrderErrors.NotFound);

        return Result<OrderResponse>.Ok(OrderMapping.ToResponse(order));
    }
}
