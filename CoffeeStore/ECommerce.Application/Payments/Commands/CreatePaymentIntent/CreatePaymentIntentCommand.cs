using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Payments.Dtos;

namespace ECommerce.Application.Payments.Commands.CreatePaymentIntent;

public sealed record CreatePaymentIntentCommand(Guid OrderId, string Email)
    : IRequest<Result<PaymentResponse>>;
