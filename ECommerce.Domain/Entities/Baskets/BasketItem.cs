using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Entities.Baskets
{
    public class BasketItem 
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;

        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
