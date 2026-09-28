using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Common
{
    public class ProductQueryParams
    {
        public string? SearchName { get; set; }

        public ProductSortOptions? Sort { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
    public enum ProductSortOptions
    {
        none = 0,
        nameAsc = 1,
        nameDsc = 2,
        priceAsc = 3,
        priceDsc = 4
    }
}
