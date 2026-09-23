using CoffeeStore.Domain.Entities.Products;
using ECommerce.Application.Specifications;

public class ProductBySlugWithCategorySpecification : BaseSpecification<Product>
{
    public ProductBySlugWithCategorySpecification(string slug)
        : base(p => p.Slug == slug)
    {
        AddInclude(p => p.Category);
    }
}