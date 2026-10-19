using FluentValidation;

namespace ECommerce.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("Order.Id.Required")
            .WithMessage("Order id is required");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithErrorCode("Order.Status.Invalid")
            .WithMessage("Order status is invalid");
    }
}
