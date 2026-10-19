using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Dtos;

namespace ECommerce.Application.DeliveryMethods.Commands.UpdateDeliveryMethod;

public sealed record UpdateDeliveryMethodCommand(
    Guid Id,
    string Name,
    decimal Price,
    string EstimatedDeliveryTime,
    string? Description,
    bool IsAvailable,
    int DisplayOrder) : IRequest<Result<DeliveryMethodResponse>>;
