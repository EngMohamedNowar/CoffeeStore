using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities.Products;

namespace ECommerce.Application.Specifications;

public class ProductBySlugWithCategorySpecification : BaseSpecification<Product>
{
    public ProductBySlugWithCategorySpecification(string slug)
        : base(p => p.Slug == slug)
    {
        AddInclude(p => p.Category);
    }
}
