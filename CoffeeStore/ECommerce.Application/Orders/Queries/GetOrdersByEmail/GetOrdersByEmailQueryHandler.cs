using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Orders.Specification;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Orders;
using FluentValidation;

namespace ECommerce.Application.Orders.Queries.GetOrdersByEmail;

internal class GetOrdersByEmailQueryHandler(
    IReadRepository<Order> repository,
    IValidator<GetOrdersByEmailQuery> validator)
    : IQueryHandler<GetOrdersByEmailQuery, Result<IReadOnlyList<OrderResponse>>>
{
    public async Task<Result<IReadOnlyList<OrderResponse>>> HandleAsync(
        GetOrdersByEmailQuery query,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<IReadOnlyList<OrderResponse>>.Fail(validation.Errors
                .Select(e => Error.Validation(
                    e.ErrorCode,
                    e.ErrorMessage))
                .ToArray());
        }
        var orders = await repository.GetAllAsync(
            new OrdersByEmailSpecification(query.Email),
            cancellationToken);

        var response = orders.Select(OrderMapping.ToResponse).ToList();

        return Result<IReadOnlyList<OrderResponse>>.Ok(response);
    }
}
