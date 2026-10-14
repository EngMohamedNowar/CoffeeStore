using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Dtos;

namespace ECommerce.Application.DeliveryMethods.Queries.GetAllDeliveryMethods;

public sealed record GetAllDeliveryMethodsQuery : IQuery<Result<IReadOnlyList<DeliveryMethodResponse>>>;
