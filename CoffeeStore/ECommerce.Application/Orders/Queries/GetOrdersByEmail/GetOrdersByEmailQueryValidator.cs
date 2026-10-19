using FluentValidation;

namespace ECommerce.Application.Orders.Queries.GetOrdersByEmail;

public class GetOrdersByEmailQueryValidator : AbstractValidator<GetOrdersByEmailQuery>
{
    public GetOrdersByEmailQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("Order.Email.Required")
            .WithMessage("Email is required")
            .EmailAddress()
            .WithErrorCode("Order.Email.Invalid")
            .WithMessage("Email is invalid");
    }
}
