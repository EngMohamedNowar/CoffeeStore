using CoffeeStore.Application.DTOs.Products;
using CoffeeStore.Domain.Entities.Enums;
using FluentValidation;

namespace CoffeeStore.Application.Validators.Products;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("اسم المنتج مطلوب")
            .MaximumLength(150);

        RuleFor(p => p.Description)
            .NotEmpty().WithMessage("الوصف مطلوب")
            .MaximumLength(2000);

        RuleFor(p => p.Origin)
            .NotEmpty().WithMessage("بلد المنشأ مطلوب")
            .MaximumLength(100);

        RuleFor(p => p.RoastLevel)
            .NotEmpty()
            .Must(BeAValidRoastLevel)
            .WithMessage("درجة التحميص غير صحيحة");

        RuleFor(p => p.CategoryId)
            .NotEmpty().WithMessage("لازم تختار قسم للمنتج");

        RuleFor(p => p.Variants)
            .NotEmpty().WithMessage("لازم يكون فيه Variant واحد على الأقل");

        RuleForEach(p => p.Variants)
            .SetValidator(new CreateProductVariantDtoValidator());

        // معمول SKU مكرر جوه نفس الطلب
        RuleFor(p => p.Variants)
            .Must(HaveUniqueSkus)
            .WithMessage("مينفعش يتكرر نفس الـ SKU في أكتر من Variant")
            .When(p => p.Variants.Any());
    }

    private static bool BeAValidRoastLevel(string value)
        => Enum.TryParse<RoastLevel>(value, ignoreCase: true, out _);

    private static bool HaveUniqueSkus(List<CreateProductVariantDto> variants)
        => variants.Select(v => v.Sku).Distinct().Count() == variants.Count;
}