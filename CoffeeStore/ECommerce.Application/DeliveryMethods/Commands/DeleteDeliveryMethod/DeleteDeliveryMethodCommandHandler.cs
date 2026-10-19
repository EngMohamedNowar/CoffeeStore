using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Specification;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Errors;

namespace ECommerce.Application.DeliveryMethods.Commands.DeleteDeliveryMethod;

internal class DeleteDeliveryMethodCommandHandler(
    IReadRepository<DeliveryMethod> readRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteDeliveryMethodCommand, Result<bool>>
{
    public async Task<Result<bool>> HandleAsync(
        DeleteDeliveryMethodCommand command,
        CancellationToken cancellationToken = default)
    {
        var deliveryMethod = await readRepository.GetBySpecAsync(
            new DeliveryMethodByIdSpecification(command.Id),
            cancellationToken);

        if (deliveryMethod is null)
            return Result<bool>.Fail(DeliveryMethodErrors.NotFound);

        unitOfWork.GetRepository<DeliveryMethod>().Delete(deliveryMethod);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
}
