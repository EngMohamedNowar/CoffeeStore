using CoffeeStore.Domain.Entities.Products;
using ECommerce.Application.Common;
using System;

namespace ECommerce.Application.Specifications
{
    public class ProductsWithCategory : BaseSpecification<Product>
    {
        public ProductsWithCategory(ProductQueryParams queryParams) : base(p => (string.IsNullOrWhiteSpace(queryParams.SearchName) || p.Name.ToLower().Contains(queryParams.SearchName.ToLower())))
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.Variants);

            switch (queryParams.Sort)
            {
                case ProductSortOptions.nameAsc:
                    AddOrderBy(p => p.Name);
                    break;

                case ProductSortOptions.nameDsc:
                    AddOrderByDescending(p => p.Name);
                    break;

                case ProductSortOptions.priceAsc:
                    AddOrderBy(
                        p => p.Variants.Min(v => v.Price));
                    break;

                case ProductSortOptions.priceDsc:
                    AddOrderByDescending(
                        p => p.Variants.Max(v => v.Price));
                    break;
            }

            ApplyPaging(queryParams.PageIndex, queryParams.PageSize);
        }
    }
}