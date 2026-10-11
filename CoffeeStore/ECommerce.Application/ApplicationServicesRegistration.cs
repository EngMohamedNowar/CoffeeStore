using AutoMapper;
using ECommerce.Application.MappingProfiles.Products;
using ECommerce.Application.Services.Classes.Authentications;using ECommerce.Application.Services.Classes.Baskets;
using ECommerce.Application.Services.Classes.Cache;
using ECommerce.Application.Services.Classes.Orders;
using ECommerce.Application.Services.Classes.Products;
using ECommerce.Application.Services.Contracts;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.Application;

public static class ApplicationServicesRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ProductProfile>();
        services.AddAutoMapper(BuildMapperConfiguration, Array.Empty<Assembly>());
        services.AddValidatorsFromAssembly(typeof(ApplicationServicesRegistration).Assembly);

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IOrderServices, OrderServices>();
        services.AddScoped<ICacheServices, CacheServices>();
        services.AddScoped<IAuthenticationService, AuthenticationsServices>();
        services.AddScoped<ITokenServices, TokenServices>();

        return services;
    }

    private static void BuildMapperConfiguration(IServiceProvider sp, IMapperConfigurationExpression cfg)
    {
        cfg.AddProfile(sp.GetRequiredService<ProductProfile>());
    }
}
