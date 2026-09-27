using AutoMapper;
using ECommerce.Application.Common;
using ECommerce.Application.MappingProfiles.Baskets;
using ECommerce.Application.MappingProfiles.Products;
using ECommerce.Application.Services.Classes.Products;
using ECommerce.Application.Specifications;
using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Categories;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Products;
using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace ECommerce.Tests.Application;

public class ProductServiceTests
{
    private static (StoreDbContext context, ProductService service) BuildService(int productCount = 12)
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new StoreDbContext(options);
        var category = new Category { Name = "Beans", Slug = "beans" };
        context.Categories.Add(category);
        context.SaveChanges();

        for (var i = 1; i <= productCount; i++)
        {
            var product = new Product
            {
                Name = $"Coffee {i:D2}",
                Slug = $"coffee-{i:D2}",
                Description = "desc",
                Origin = "Ethiopia",
                RoastLevel = RoastLevel.Medium,
                IsActive = true,
                CategoryId = category.Id
            };

            product.Variants.Add(new ProductVariant
            {
                WeightInGrams = 250,
                GrindType = GrindType.WholeBean,
                Price = 50 + i,
                StockQuantity = 5,
                Sku = $"SKU-{i:D2}"
            });

            context.Products.Add(product);
        }

        context.SaveChanges();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["BaseUrl"] = "https://localhost:7135" })
            .Build();

        var mapper = new MapperConfiguration(
                cfg => cfg.AddProfile(new ProductProfile(configuration)),
                NullLoggerFactory.Instance)
            .CreateMapper();

        return (context, new ProductService(new TestUnitOfWork(context), mapper));
    }

    [Fact]
    public async Task GetAllActiveAsync_ReturnsOnlyTheRequestedPage()
    {
        var (context, service) = BuildService();
        await using var _ = context;

        var result = await service.GetAllActiveAsync(new ProductQueryParams { PageIndex = 2, PageSize = 5 });

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value!.Data.Count);
    }

    [Fact]
    public async Task GetAllActiveAsync_ReportsTheTotalMatchCountNotThePageSize()
    {
        // Regression: the count used to be evaluated after Skip/Take, so it reported 5
        // for every page instead of 12.
        var (context, service) = BuildService();
        await using var _ = context;

        var result = await service.GetAllActiveAsync(new ProductQueryParams { PageIndex = 1, PageSize = 5 });

        Assert.Equal(12, result.Value!.Count);
    }

    [Fact]
    public async Task GetAllActiveAsync_TotalCountIsStableAcrossPages()
    {
        var (context, service) = BuildService();
        await using var _ = context;

        var first = await service.GetAllActiveAsync(new ProductQueryParams { PageIndex = 1, PageSize = 5 });
        var last = await service.GetAllActiveAsync(new ProductQueryParams { PageIndex = 3, PageSize = 5 });

        Assert.Equal(first.Value!.Count, last.Value!.Count);
        Assert.Equal(2, last.Value.Data.Count);
    }

    [Fact]
    public async Task GetAllActiveAsync_FiltersBySearchName()
    {
        var (context, service) = BuildService();
        await using var _ = context;

        var result = await service.GetAllActiveAsync(new ProductQueryParams
        {
            SearchName = "Coffee 01",
            PageIndex = 1,
            PageSize = 50
        });

        Assert.Equal("Coffee 01", Assert.Single(result.Value!.Data).Name);
        Assert.Equal(1, result.Value.Count);
    }

    [Fact]
    public async Task GetAllActiveAsync_SortsByNameAscending()
    {
        var (context, service) = BuildService();
        await using var _ = context;

        var result = await service.GetAllActiveAsync(new ProductQueryParams
        {
            Sort = ProductSortOptions.nameAsc,
            PageIndex = 1,
            PageSize = 50
        });

        var names = result.Value!.Data.Select(i => i.Name).ToList();
        Assert.Equal(names.OrderBy(n => n, StringComparer.Ordinal), names);
    }

    [Fact]
    public async Task GetAllActiveAsync_ExcludesSoftDeletedProducts()
    {
        var (context, service) = BuildService();
        await using var _ = context;

        var product = await context.Products.FirstAsync(p => p.Slug == "coffee-02");
        product.IsDeleted = true;
        await context.SaveChangesAsync();

        var result = await service.GetAllActiveAsync(new ProductQueryParams { PageIndex = 1, PageSize = 50 });

        Assert.Equal(11, result.Value!.Count);
        Assert.DoesNotContain(result.Value.Data, p => p.Slug == "coffee-02");
    }

    [Fact]
    public async Task CountAsync_IgnoresPagingCarriedByTheSpecification()
    {
        // End-to-end guard for the paging bug: a count spec that happens to carry
        // ApplyPaging must still report the full match count, not min(total, pageSize).
        var (context, _) = BuildService();
        await using var _ = context;

        var repository = new GenericRepository<Product>(context);
        var queryParams = new ProductQueryParams { PageIndex = 1, PageSize = 4 };
        var pagedSpec = new ProductsWithCategory(queryParams);

        var count = await repository.CountAsync(pagedSpec);

        Assert.Equal(12, count);
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsTheProductWhenItExists()
    {
        var (context, service) = BuildService();
        await using var _ = context;

        var result = await service.GetBySlugAsync("coffee-03");

        Assert.True(result.IsSuccess);
        Assert.Equal("Coffee 03", result.Value!.Name);
    }

    [Fact]
    public async Task GetBySlugAsync_FailsWithNotFoundForAnUnknownSlug()
    {
        // Regression: the service used to return 200 with a null body.
        var (context, service) = BuildService();
        await using var _ = context;

        var result = await service.GetBySlugAsync("does-not-exist");

        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorType.NotFound, error.ErrorType);
    }

    [Fact]
    public async Task GetBySlugAsync_FailsWithNotFoundForASoftDeletedProduct()
    {
        var (context, service) = BuildService();
        await using var _ = context;

        var product = await context.Products.FirstAsync(p => p.Slug == "coffee-02");
        product.IsDeleted = true;
        await context.SaveChangesAsync();

        var result = await service.GetBySlugAsync("coffee-02");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, Assert.Single(result.Errors).ErrorType);
    }

    private sealed class TestUnitOfWork(StoreDbContext context) : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories = [];

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            if (_repositories.TryGetValue(typeof(TEntity), out var existing))
            {
                return (IGenericRepository<TEntity>)existing;
            }

            var repository = new GenericRepository<TEntity>(context);
            _repositories[typeof(TEntity)] = repository;
            return repository;
        }

        public IProductRepository ProductRepository() => new ProductRepository(context);

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
    }
}
