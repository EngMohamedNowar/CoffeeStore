using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Orders.Specification;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Errors;
using FluentValidation;

namespace ECommerce.Application.Orders.Commands.CancelOrder;

internal class CancelOrderCommandHandler(
    IReadRepository<Order> readRepository,
    IUnitOfWork unitOfWork,
    IValidator<CancelOrderCommand> validator)
    : IRequestHandler<CancelOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> HandleAsync(
        CancelOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<OrderResponse>.Fail(validation.Errors
                .Select(e => Error.Validation(
                    e.ErrorCode,
                    e.ErrorMessage))
                .ToArray());
        }

        var order = await readRepository.GetBySpecAsync(
            new OrderByIdSpecification(command.Id),
            cancellationToken);

        if (order is null)
            return Result<OrderResponse>.Fail(OrderErrors.NotFound);

        if (!string.Equals(
                order.UserEmail,
                command.Email.Trim(),
                StringComparison.OrdinalIgnoreCase))
            return Result<OrderResponse>.Fail(OrderErrors.Forbidden);

        var cancelResult = order.Cancel();
        if (cancelResult.IsFailure)
            return Result<OrderResponse>.Fail(cancelResult.Errors);

        unitOfWork.GetRepository<Order>().Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrderResponse>.Ok(OrderMapping.ToResponse(order));
    }
}
