using ECommerce.Application.Common.Messaging;

namespace ECommerce.Application.DeliveryMethods.Commands.DeleteDeliveryMethod;

public sealed record DeleteDeliveryMethodCommand(Guid Id) : IRequest<Result<bool>>;
