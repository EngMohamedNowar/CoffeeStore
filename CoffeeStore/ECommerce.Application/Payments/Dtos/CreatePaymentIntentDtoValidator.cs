using FluentValidation;

namespace ECommerce.Application.Payments.Dtos;

public class CreatePaymentIntentDtoValidator : AbstractValidator<CreatePaymentIntentDto>
{
    public CreatePaymentIntentDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithErrorCode("Payment.OrderId.Required")
            .WithMessage("Order id is required");
    }
}
