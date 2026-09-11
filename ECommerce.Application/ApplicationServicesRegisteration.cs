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
            return services;
        }
    }
}
