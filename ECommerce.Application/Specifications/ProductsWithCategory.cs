using CoffeeStore.Domain.Entities.Products;
using System;

namespace ECommerce.Application.Specifications
{
    public class ProductsWithCategory : BaseSpecification<Product>
    {
        public ProductsWithCategory()
        {
            AddInclude(p => p.Category);
        }

        public ProductsWithCategory(string slug) : base(p => p.Slug == slug)
        {
            AddInclude(p => p.Category);
        }
    }
}