using FluentValidation;

namespace ECommerce.Application.Payments.Queries.GetPaymentByOrderId;

public class GetPaymentByOrderIdQueryValidator : AbstractValidator<GetPaymentByOrderIdQuery>
{
    public GetPaymentByOrderIdQueryValidator()
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
