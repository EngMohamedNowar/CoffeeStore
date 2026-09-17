using CoffeeStore.Domain.Entities.Products;
using ECommerce.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Specifications
{
    public class ProductCountSpecifications : BaseSpecification<Product>
    {


        public ProductCountSpecifications(ProductQueryParams queryParams) : base(p => (string.IsNullOrWhiteSpace(queryParams.SearchName) || p.Name.ToLower().Contains(queryParams.SearchName.ToLower())))
        {
        }
    }
}