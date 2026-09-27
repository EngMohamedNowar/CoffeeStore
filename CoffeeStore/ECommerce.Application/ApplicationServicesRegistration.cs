using ECommerce.Application.Services.Classes.Authentications;
using ECommerce.Application.Services.Classes.Baskets;
using ECommerce.Application.Services.Classes.Cache;
using ECommerce.Application.Services.Classes.Products;
using ECommerce.Application.Services.Contracts;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application;

public static class ApplicationServicesRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(_ => { }, typeof(ApplicationServicesRegistration).Assembly);
        services.AddValidatorsFromAssembly(typeof(ApplicationServicesRegistration).Assembly);

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<ICacheServices, CacheServices>();
        services.AddScoped<IAuthenticationService, AuthenticationsServices>();
        services.AddScoped<ITokenServices, TokenServices>();

        return services;
    }
}
