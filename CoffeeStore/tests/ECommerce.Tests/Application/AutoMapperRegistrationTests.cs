using AutoMapper;
using ECommerce.Application;
using ECommerce.Application.DTOs.Products;
using ECommerce.Domain.Entities.Categories;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Products;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Tests.Application;

public class AutoMapperRegistrationTests
{
    [Fact]
    public void AppStyleRegistration_StillResolvesBaseUrl()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BaseUrl"] = "https://cdn.example.com"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddApplicationServices(configuration);

        using var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<IMapper>();

        var category = new Category { Id = Guid.NewGuid(), Name = "Beans", Slug = "beans" };
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Slug = "test",
            Description = "d",
            Origin = "Ethiopia",
            RoastLevel = RoastLevel.Light,
            ImageUrl = "images/test.png",
            CategoryId = category.Id,
            Category = category
        };

        var dto = mapper.Map<ProductDto>(product);

        Assert.Equal("https://cdn.example.com/images/test.png", dto.ImageUrl);
    }
}
