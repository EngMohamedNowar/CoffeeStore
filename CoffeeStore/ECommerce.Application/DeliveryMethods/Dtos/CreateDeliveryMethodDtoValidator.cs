using ECommerce.Domain.Entities.Orders;
using FluentValidation;

namespace ECommerce.Application.DeliveryMethods.Dtos;

public class CreateDeliveryMethodDtoValidator : AbstractValidator<CreateDeliveryMethodDto>
{
    public CreateDeliveryMethodDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode("DeliveryMethod.Name.Required")
            .WithMessage("Delivery method name is required")
            .MaximumLength(DeliveryMethod.MaxNameLength)
            .WithErrorCode("DeliveryMethod.Name.TooLong")
            .WithMessage($"Delivery method name cannot exceed {DeliveryMethod.MaxNameLength} characters");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode("DeliveryMethod.Price.Invalid")
            .WithMessage("Delivery price cannot be negative");

        RuleFor(x => x.EstimatedDeliveryTime)
            .NotEmpty()
            .WithErrorCode("DeliveryMethod.EstimatedDeliveryTime.Required")
            .WithMessage("Estimated delivery time is required")
            .MaximumLength(DeliveryMethod.MaxDeliveryTimeLength)
            .WithErrorCode("DeliveryMethod.EstimatedDeliveryTime.TooLong")
            .WithMessage($"Estimated delivery time cannot exceed {DeliveryMethod.MaxDeliveryTimeLength} characters");

        RuleFor(x => x.Description)
            .MaximumLength(DeliveryMethod.MaxDescriptionLength)
            .WithErrorCode("DeliveryMethod.Description.TooLong")
            .WithMessage($"Delivery method description cannot exceed {DeliveryMethod.MaxDescriptionLength} characters");
    }
}
