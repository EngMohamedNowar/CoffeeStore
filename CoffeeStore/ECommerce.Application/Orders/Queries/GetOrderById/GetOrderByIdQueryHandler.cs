using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Orders.Specification;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Errors;
using FluentValidation;

namespace ECommerce.Application.Orders.Queries.GetOrderById;

internal class GetOrderByIdQueryHandler(
    IReadRepository<Order> repository,
    IValidator<GetOrderByIdQuery> validator)
    : IQueryHandler<GetOrderByIdQuery, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> HandleAsync(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<OrderResponse>.Fail(validation.Errors
                .Select(e => Error.Validation(
                    e.ErrorCode,
                    e.ErrorMessage))
                .ToArray());
        }
        var order = await repository.GetBySpecAsync(
            new OrderByIdSpecification(query.Id),
            cancellationToken);

        if (order is null)
            return Result<OrderResponse>.Fail(OrderErrors.NotFound);

        if (!query.IsAdmin
            && !string.Equals(
                order.UserEmail,
                query.Email.Trim(),
                StringComparison.OrdinalIgnoreCase))
            return Result<OrderResponse>.Fail(OrderErrors.Forbidden);

        return Result<OrderResponse>.Ok(OrderMapping.ToResponse(order));
    }
}
