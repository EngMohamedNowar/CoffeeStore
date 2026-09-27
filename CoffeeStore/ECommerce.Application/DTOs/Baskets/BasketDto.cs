using System;
using System.Collections.Generic;

namespace ECommerce.Application.DTOs.Baskets
{
    public class BasketDto
    {
        public Guid Id { get; set; }

        public ICollection<BasketItemDto> Items { get; set; }
            = new List<BasketItemDto>();
    }
}
