using ECommerce.Domain.Entities.Products;

namespace ECommerce.Application.Specifications;

public class ProductVariantsByIdsSpecification : BaseSpecification<ProductVariant>
{
    public ProductVariantsByIdsSpecification(IReadOnlyCollection<Guid> ids)
        : base(v => ids.Contains(v.Id))
    {
    }
}
