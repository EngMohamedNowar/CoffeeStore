using FluentValidation;

namespace ECommerce.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("Order.Id.Required")
            .WithMessage("Order id is required");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("Order.Email.Required")
            .WithMessage("Email is required")
            .EmailAddress()
            .WithErrorCode("Order.Email.Invalid")
            .WithMessage("Email is invalid");
    }
}
