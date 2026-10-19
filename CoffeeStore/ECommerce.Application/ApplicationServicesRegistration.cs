using AutoMapper;
using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Commands.CreateDeliveryMethod;
using ECommerce.Application.DeliveryMethods.Commands.DeleteDeliveryMethod;
using ECommerce.Application.DeliveryMethods.Commands.UpdateDeliveryMethod;
using ECommerce.Application.DeliveryMethods.Dtos;
using ECommerce.Application.DeliveryMethods.Queries.GetAllDeliveryMethods;
using ECommerce.Application.DeliveryMethods.Queries.GetDeliveryMethodById;
using ECommerce.Application.MappingProfiles.Baskets;
using ECommerce.Application.MappingProfiles.DeliveryMethods;
using ECommerce.Application.MappingProfiles.Products;
using ECommerce.Application.Orders.Commands.CancelOrder;
using ECommerce.Application.Orders.Commands.CreateOrder;
using ECommerce.Application.Orders.Commands.UpdateOrder;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Orders.Queries.GetOrderById;
using ECommerce.Application.Orders.Queries.GetOrdersByEmail;
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
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<IRequestDispatcher, RequestDispatcher>();
        services.AddScoped<IQueryHandler<GetDeliveryMethodByIdQuery, Result<DeliveryMethodResponse>>, GetDeliveryMethodByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllDeliveryMethodsQuery, Result<IReadOnlyList<DeliveryMethodResponse>>>, GetAllDeliveryMethodsQueryHandler>();
        services.AddScoped<IRequestHandler<CreateDeliveryMethodCommand, Result<DeliveryMethodResponse>>, CreateDeliveryMethodCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateDeliveryMethodCommand, Result<DeliveryMethodResponse>>, UpdateDeliveryMethodCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteDeliveryMethodCommand, Result<bool>>, DeleteDeliveryMethodCommandHandler>();
        services.AddScoped<IRequestHandler<CreateOrderCommand, Result<OrderResponse>>, CreateOrderCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateOrderCommand, Result<OrderResponse>>, UpdateOrderCommandHandler>();
        services.AddScoped<IRequestHandler<CancelOrderCommand, Result<OrderResponse>>, CancelOrderCommandHandler>();
        services.AddScoped<IQueryHandler<GetOrdersByEmailQuery, Result<IReadOnlyList<OrderResponse>>>, GetOrdersByEmailQueryHandler>();
        services.AddScoped<IQueryHandler<GetOrderByIdQuery, Result<OrderResponse>>, GetOrderByIdQueryHandler>();
        services.AddScoped<IAuthenticationService, AuthenticationsServices>();
        services.AddScoped<ITokenServices, TokenServices>();

        return services;
    }

    private static void BuildMapperConfiguration(IServiceProvider sp, IMapperConfigurationExpression cfg)
    {
        cfg.AddProfile(sp.GetRequiredService<ProductProfile>());
        cfg.AddProfile(new BasketsProfile());
        cfg.AddProfile(new DeliveryMethodProfile());
    }
}
