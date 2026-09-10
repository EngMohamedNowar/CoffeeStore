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
            return services;
        }
    }
}
