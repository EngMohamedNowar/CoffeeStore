using FluentValidation;

namespace ECommerce.Application.Payments.Commands.HandlePaymentWebhook;

public class HandlePaymentWebhookCommandValidator : AbstractValidator<HandlePaymentWebhookCommand>
{
    public HandlePaymentWebhookCommandValidator()
    {
        RuleFor(x => x.JsonPayload)
            .NotEmpty()
            .WithErrorCode("Payment.Payload.Required")
            .WithMessage("Webhook payload is required");

        RuleFor(x => x.StripeSignature)
            .NotEmpty()
            .WithErrorCode("Payment.Signature.Required")
            .WithMessage("Webhook signature is required");
    }
}
