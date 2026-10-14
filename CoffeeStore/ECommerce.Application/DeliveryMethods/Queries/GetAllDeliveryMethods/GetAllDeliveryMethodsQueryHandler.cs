using AutoMapper;
using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Dtos;
using ECommerce.Application.DeliveryMethods.Specification;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.DeliveryMethods.Queries.GetAllDeliveryMethods;

internal class GetAllDeliveryMethodsQueryHandler(
    IReadRepository<DeliveryMethod> repository,
    IMapper mapper)
    : IQueryHandler<GetAllDeliveryMethodsQuery, Result<IReadOnlyList<DeliveryMethodResponse>>>
{
    public async Task<Result<IReadOnlyList<DeliveryMethodResponse>>> HandleAsync(
        GetAllDeliveryMethodsQuery query,
        CancellationToken cancellationToken = default)
    {
        var deliveryMethods = await repository.GetAllAsync(
            new AllDeliveryMethodsSpecification(),
            cancellationToken);

        var response = mapper.Map<IReadOnlyList<DeliveryMethodResponse>>(deliveryMethods);

        return Result<IReadOnlyList<DeliveryMethodResponse>>.Ok(response);
    }
}
