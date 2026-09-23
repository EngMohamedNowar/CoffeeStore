using AutoMapper;
using ECommerce.Application.DTOs.Baskets;
using ECommerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.MappingProfiles.Baskets
{
    public class BasketsProfile :Profile
    {
        public BasketsProfile()
        {
            CreateMap<BasketCustomer, BasketDto>().ReverseMap();
            CreateMap<BasketItem, BasketItemDto>().ReverseMap();

        }
    }
}
