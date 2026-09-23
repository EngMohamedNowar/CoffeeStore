using ECommerce.Application.Services.Classes.Baskets;
using ECommerce.Application.Services.Classes.Cache;
using ECommerce.Application.Services.Classes.Products;
using ECommerce.Application.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application
{
    public static class ApplicationServicesRegisteration
    {
        public static IServiceCollection AddApplictaionServices(this IServiceCollection services)
        {
            services.AddAutoMapper(c => { }, typeof(ApplicationServicesRegisteration).Assembly);
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<ICacheServices, CacheServices>();
            return services;
        }
    }
}
