using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Dtos;

namespace ECommerce.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid BasketId,
    Guid DeliveryMethodId,
    string Email,
    string FirstName,
    string LastName,
    string Street,
    string City,
    string Country) : IRequest<Result<OrderResponse>>;
