using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Identity;
using ECommerce.Infrastructure.Identity.Services;
using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Persistence.DataSeeding;
using ECommerce.Infrastructure.Persistence.Identity.Data;
using ECommerce.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ECommerce.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddDbContext<StoreIdentityDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
        });

        services.AddKeyedScoped<IDataSeeder, CatalogDataSeeder>("Catalog");
        services.AddKeyedScoped<IDataSeeder, IdentityDataSeeder>("Identity");

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("RedisConnection")
                ?? throw new InvalidOperationException("ConnectionStrings:RedisConnection is not configured.")));

        services.AddIdentityCore<ApplicationUser>(options => { options.User.RequireUniqueEmail = true; })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<StoreIdentityDbContext>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<ICacheRepository, CacheRepository>();
        services.AddScoped<IIdentitityServices, IdentityServices>();

        return services;
    }
}
