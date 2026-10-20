using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Errors;
using FluentValidation;

namespace ECommerce.Application.Payments.Commands.HandlePaymentWebhook;

internal class HandlePaymentWebhookCommandHandler(
    IPaymentService paymentService,
    IValidator<HandlePaymentWebhookCommand> validator)
    : IRequestHandler<HandlePaymentWebhookCommand, Result<bool>>
{
    public async Task<Result<bool>> HandleAsync(
        HandlePaymentWebhookCommand command,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<bool>.Fail(validation.Errors
                .Select(e => Error.Validation(e.ErrorCode, e.ErrorMessage))
                .ToArray());
        }

        var parseResult = paymentService.ParseWebhook(command.JsonPayload, command.StripeSignature);
        if (parseResult.IsFailure)
            return Result<bool>.Fail(parseResult.Errors);

        var webhookEvent = parseResult.Value!;

        switch (webhookEvent.EventType)
        {
            case "payment_intent.succeeded":
                await paymentService.PaymentSucceeded(webhookEvent.PaymentIntentId, cancellationToken);
                break;

            case "payment_intent.payment_failed":
                await paymentService.PaymentFailed(webhookEvent.PaymentIntentId, cancellationToken);
                break;
        }

        return Result<bool>.Ok(true);
    }
}
