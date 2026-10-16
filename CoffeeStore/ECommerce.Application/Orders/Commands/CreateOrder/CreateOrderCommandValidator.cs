using FluentValidation;

namespace ECommerce.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.BasketId)
            .NotEmpty()
            .WithErrorCode("Order.BasketId.Required")
            .WithMessage("Basket id is required");

        RuleFor(x => x.DeliveryMethodId)
            .NotEmpty()
            .WithErrorCode("Order.DeliveryMethodId.Required")
            .WithMessage("Delivery method id is required");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("Order.Email.Required")
            .WithMessage("Email is required")
            .EmailAddress()
            .WithErrorCode("Order.Email.Invalid")
            .WithMessage("Email is invalid");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithErrorCode("Order.FirstName.Required")
            .WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithErrorCode("Order.LastName.Required")
            .WithMessage("Last name is required");

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithErrorCode("Order.Street.Required")
            .WithMessage("Street is required");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithErrorCode("Order.City.Required")
            .WithMessage("City is required");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithErrorCode("Order.Country.Required")
            .WithMessage("Country is required");
    }
}
