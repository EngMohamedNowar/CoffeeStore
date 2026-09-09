using CoffeeStore.Application.DTOs.Products;
using CoffeeStore.Domain.Entities.Enums;
using FluentValidation;

namespace CoffeeStore.Application.Validators.Products;

public class CreateProductVariantDtoValidator : AbstractValidator<CreateProductVariantDto>
{
    public CreateProductVariantDtoValidator()
    {
        RuleFor(v => v.WeightInGrams)
            .GreaterThan(0).WithMessage("الوزن لازم يكون أكبر من صفر");

        RuleFor(v => v.GrindType)
            .NotEmpty()
            .Must(BeAValidGrindType)
            .WithMessage("نوع الطحن غير صحيح");

        RuleFor(v => v.Price)
            .GreaterThan(0).WithMessage("السعر لازم يكون أكبر من صفر");

        RuleFor(v => v.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("الكمية متقدرش تكون سالبة");

        RuleFor(v => v.Sku)
            .NotEmpty().WithMessage("SKU مطلوب")
            .MaximumLength(50);
    }

    private static bool BeAValidGrindType(string value)
        => Enum.TryParse<GrindType>(value, ignoreCase: true, out _);
}