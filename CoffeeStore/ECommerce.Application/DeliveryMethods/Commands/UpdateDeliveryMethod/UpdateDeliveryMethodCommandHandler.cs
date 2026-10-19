using AutoMapper;
using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Dtos;
using ECommerce.Application.DeliveryMethods.Specification;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Errors;

namespace ECommerce.Application.DeliveryMethods.Commands.UpdateDeliveryMethod;

internal class UpdateDeliveryMethodCommandHandler(
    IReadRepository<DeliveryMethod> readRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<UpdateDeliveryMethodCommand, Result<DeliveryMethodResponse>>
{
    public async Task<Result<DeliveryMethodResponse>> HandleAsync(
        UpdateDeliveryMethodCommand command,
        CancellationToken cancellationToken = default)
    {
        var deliveryMethod = await readRepository.GetBySpecAsync(
            new DeliveryMethodByIdSpecification(command.Id),
            cancellationToken);

        if (deliveryMethod is null)
            return Result<DeliveryMethodResponse>.Fail(DeliveryMethodErrors.NotFound);

        var nameTaken = await readRepository.CountAsync(
            new DeliveryMethodByNameSpecification(command.Name),
            cancellationToken);

        if (nameTaken > 0 && !string.Equals(
                deliveryMethod.Name, command.Name.Trim(), StringComparison.OrdinalIgnoreCase))
            return Result<DeliveryMethodResponse>.Fail(DeliveryMethodErrors.NameAlreadyExists);

        var updateResult = deliveryMethod.Update(
            command.Name,
            command.Price,
            command.EstimatedDeliveryTime,
            command.Description,
            command.IsAvailable,
            command.DisplayOrder);

        if (updateResult.IsFailure)
            return Result<DeliveryMethodResponse>.Fail(updateResult.Errors);

        unitOfWork.GetRepository<DeliveryMethod>().Update(deliveryMethod);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DeliveryMethodResponse>.Ok(
            mapper.Map<DeliveryMethodResponse>(deliveryMethod));
    }
}
