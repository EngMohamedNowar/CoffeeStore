using AutoMapper;
using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Dtos;
using ECommerce.Application.DeliveryMethods.Specification;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Errors;

namespace ECommerce.Application.DeliveryMethods.Commands.CreateDeliveryMethod;

internal class CreateDeliveryMethodCommandHandler(
    IReadRepository<DeliveryMethod> readRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<CreateDeliveryMethodCommand, Result<DeliveryMethodResponse>>
{
    public async Task<Result<DeliveryMethodResponse>> HandleAsync(
        CreateDeliveryMethodCommand command,
        CancellationToken cancellationToken = default)
    {
        var nameTaken = await readRepository.CountAsync(
            new DeliveryMethodByNameSpecification(command.Name),
            cancellationToken);

        if (nameTaken > 0)
            return Result<DeliveryMethodResponse>.Fail(DeliveryMethodErrors.NameAlreadyExists);

        var createResult = DeliveryMethod.Create(
            Guid.NewGuid(),
            command.Name,
            command.Price,
            command.EstimatedDeliveryTime,
            command.Description,
            command.IsAvailable,
            command.DisplayOrder);

        if (createResult.IsFailure)
            return Result<DeliveryMethodResponse>.Fail(createResult.Errors);

        unitOfWork.GetRepository<DeliveryMethod>().Add(createResult.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DeliveryMethodResponse>.Ok(
            mapper.Map<DeliveryMethodResponse>(createResult.Value));
    }
}
