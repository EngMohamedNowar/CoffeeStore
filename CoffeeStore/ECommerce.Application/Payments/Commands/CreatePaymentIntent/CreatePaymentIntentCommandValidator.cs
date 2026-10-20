using FluentValidation;

namespace ECommerce.Application.Payments.Commands.CreatePaymentIntent;

public class CreatePaymentIntentCommandValidator : AbstractValidator<CreatePaymentIntentCommand>
{
    public CreatePaymentIntentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithErrorCode("Payment.OrderId.Required")
            .WithMessage("Order id is required");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("Payment.Email.Required")
            .WithMessage("Email is required")
            .EmailAddress()
            .WithErrorCode("Payment.Email.Invalid")
            .WithMessage("Email is invalid");
    }
}
