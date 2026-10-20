using ECommerce.Application.Common.Messaging;

namespace ECommerce.Application.Payments.Commands.HandlePaymentWebhook;

public sealed record HandlePaymentWebhookCommand(string JsonPayload, string StripeSignature)
    : IRequest<Result<bool>>;
