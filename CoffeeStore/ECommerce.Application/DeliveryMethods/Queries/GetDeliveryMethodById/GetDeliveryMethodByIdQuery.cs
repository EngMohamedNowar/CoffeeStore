using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Dtos;

namespace ECommerce.Application.DeliveryMethods.Queries.GetDeliveryMethodById;

public sealed class GetDeliveryMethodByIdQuery(Guid Id) : IQuery<Result<DeliveryMethodResponse>>;
