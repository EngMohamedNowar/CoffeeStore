using ECommerce.Domain.Contracts;
using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Persistence.DataSeeding;
using ECommerce.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration) 
        {
            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddKeyedScoped<IDataSeeder, CatalogDataSeeder>("Catalog");

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
