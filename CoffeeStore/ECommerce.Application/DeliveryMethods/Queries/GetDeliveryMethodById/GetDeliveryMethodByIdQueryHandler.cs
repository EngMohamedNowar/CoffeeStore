using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Dtos;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.DeliveryMethods.Queries.GetDeliveryMethodById;

internal class GetDeliveryMethodByIdQueryHandler(IReadRepository<DeliveryMethod> repository)
    : IQueryHandler<GetDeliveryMethodByIdQuery, Result<DeliveryMethodResponse>>
{
    public Task<Result<DeliveryMethodResponse>> HandleAsync(GetDeliveryMethodByIdQuery query, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
