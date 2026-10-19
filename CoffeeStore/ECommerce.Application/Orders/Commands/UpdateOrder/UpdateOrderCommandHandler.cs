using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Orders.Specification;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Errors;
using FluentValidation;

namespace ECommerce.Application.Orders.Commands.UpdateOrder;

internal class UpdateOrderCommandHandler(
    IReadRepository<Order> readRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateOrderCommand> validator)
    : IRequestHandler<UpdateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> HandleAsync(
        UpdateOrderCommand command,
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

        var changeResult = order.ChangeStatus(command.Status);
        if (changeResult.IsFailure)
            return Result<OrderResponse>.Fail(changeResult.Errors);

        unitOfWork.GetRepository<Order>().Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrderResponse>.Ok(OrderMapping.ToResponse(order));
    }
}
