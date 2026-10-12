using AutoMapper;
using ECommerce.Application;
using ECommerce.Application.DeliveryMethods.Dtos;
using ECommerce.Application.DeliveryMethods.Specification;
using ECommerce.Application.DTOs.Baskets;
using ECommerce.Application.MappingProfiles.Baskets;
using ECommerce.Application.MappingProfiles.DeliveryMethods;
using ECommerce.Domain.Entities.Baskets;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Infrastructure.Specifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace ECommerce.Tests.Application;

public class DeliveryMethodSpecAndMappingTests
{
    private static DeliveryMethod CreateMethod(Guid id, string name, decimal price = 25m)
        => DeliveryMethod.Create(id, name, price, "2-4 days", "Ships in a couple of days").Value!;

    [Fact]
    public void ByIdSpecification_ReturnsOnlyTheMatchingDeliveryMethod()
    {
        var wanted = CreateMethod(Guid.NewGuid(), "Standard");
        var other = CreateMethod(Guid.NewGuid(), "Express");
        var source = new[] { other, wanted }.AsQueryable();

        var result = SpecificationEvaluator
            .CreateQuery(source, new DeliveryMethodByIdSpecification(wanted.Id))
            .ToList();

        var matched = Assert.Single(result);
        Assert.Equal(wanted.Id, matched.Id);
        Assert.Equal("Standard", matched.Name);
    }

    [Fact]
    public void ByIdSpecification_ReturnsNothing_ForAnUnknownId()
    {
        var source = new[] { CreateMethod(Guid.NewGuid(), "Standard") }.AsQueryable();

        var result = SpecificationEvaluator
            .CreateQuery(source, new DeliveryMethodByIdSpecification(Guid.NewGuid()))
            .ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void DeliveryMethodProfile_MapsEveryFieldOntoTheResponse()
    {
        var mapper = new MapperConfiguration(
                cfg => cfg.AddProfile(new DeliveryMethodProfile()),
                NullLoggerFactory.Instance)
            .CreateMapper();

        var id = Guid.NewGuid();
        var entity = CreateMethod(id, "Express", price: 40m);

        var response = mapper.Map<DeliveryMethodResponse>(entity);

        Assert.Equal(id, response.Id);
        Assert.Equal("Express", response.Name);
        Assert.Equal("Ships in a couple of days", response.Description);
        Assert.Equal(40m, response.Price);
        Assert.Equal("2-4 days", response.EstimatedDeliveryTime);
        Assert.True(response.IsAvailable);
        Assert.Equal(entity.DisplayOrder, response.DisplayOrder);
    }

    [Fact]
    public void ApplicationServices_RegistersTheDeliveryMethodMapping()
    {
        var configuration = new ConfigurationBuilder().Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddApplicationServices(configuration);

        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<IMapper>();

        var entity = CreateMethod(Guid.NewGuid(), "Standard", price: 15m);
        var response = mapper.Map<DeliveryMethodResponse>(entity);

        Assert.Equal(15m, response.Price);
        Assert.Equal("Standard", response.Name);
    }

    [Fact]
    public void ApplicationServices_RegistersTheBasketMapping()
    {
        var configuration = new ConfigurationBuilder().Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddApplicationServices(configuration);

        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<IMapper>();

        var basket = new BasketCustomer
        {
            Id = Guid.NewGuid(),
            Items = []
        };

        var dto = mapper.Map<BasketDto>(basket);

        Assert.Equal(basket.Id, dto.Id);
        Assert.Empty(dto.Items);
    }
}
