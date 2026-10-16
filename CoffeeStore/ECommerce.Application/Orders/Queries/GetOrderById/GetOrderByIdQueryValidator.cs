using FluentValidation;

namespace ECommerce.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("Order.Id.Required")
            .WithMessage("Order id is required");
    }
}
