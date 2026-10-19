using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Dtos;

namespace ECommerce.Application.DeliveryMethods.Commands.CreateDeliveryMethod;

public sealed record CreateDeliveryMethodCommand(
    string Name,
    decimal Price,
    string EstimatedDeliveryTime,
    string? Description,
    bool IsAvailable,
    int DisplayOrder) : IRequest<Result<DeliveryMethodResponse>>;
