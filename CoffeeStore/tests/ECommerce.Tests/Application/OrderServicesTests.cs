using AutoMapper;
using ECommerce.Application.DTOs.Baskets;
using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Services.Classes.Orders;
using ECommerce.Application.Specifications;
using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Baskets;
using ECommerce.Domain.Entities.Customers;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Specification;

namespace ECommerce.Tests.Application;

public class OrderServicesTests
{
    private const string UserEmail = "ali@example.com";

    [Fact]
    public async Task CreateOrderAsync_SnapshotsTheDatabasePrice_NotTheBasketPrice()
    {
        // The basket in Redis is client-controlled: it claims a unit price of 0.01.
        var tamperedBasket = new BasketCustomer
        {
            Id = Guid.NewGuid(),
            Items =
            {
                new BasketItem(
                    productVariantId: FixedIds.Variant,
                    productName: "HACKED NAME",
                    pictureUrl: null,
                    unitPrice: 0.01m,
                    quantity: 2)
            }
        };

        var (services, orders) = BuildServices(tamperedBasket);

        var result = await services.CreateOrderAsync(
            new OrderDto
            {
                BasketId = tamperedBasket.Id,
                DeliveryMethodId = FixedIds.DeliveryMethod,
                ShipToAddress = new OrderAddressToReturn
                {
                    FirstName = "Ali",
                    LastName = "Hassan",
                    Street = "12 Main St",
                    City = "Cairo",
                    Country = "Egypt"
                }
            },
            UserEmail,
            default);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));

        var item = Assert.Single(result.Value!.Items);
        Assert.Equal(50m, item.UnitPrice);
        Assert.Equal(100m, item.LineTotal);
        Assert.Equal(100m, result.Value.SubTotal);

        var saved = Assert.Single(orders);
        var savedItem = Assert.Single(saved.Items);
        Assert.Equal(50m, savedItem.ItemOrdered.UnitPrice);
        Assert.Equal("Ethiopian Yirgacheffe", savedItem.ItemOrdered.ProductName);
        Assert.Equal(250, savedItem.ItemOrdered.WeightInGrams);
        Assert.Equal(FixedIds.Variant, savedItem.ItemOrdered.ProductVariantId);
    }

    [Fact]
    public async Task CreateOrderAsync_FailsWhenTheQuantityExceedsTheStock()
    {
        // Stock is 10, the basket asks for 11.
        var basket = new BasketCustomer
        {
            Id = Guid.NewGuid(),
            Items =
            {
                new BasketItem(FixedIds.Variant, "Ethiopian Yirgacheffe", null, 50m, quantity: 11)
            }
        };

        var (services, orders) = BuildServices(basket);

        var result = await services.CreateOrderAsync(
            new OrderDto
            {
                BasketId = basket.Id,
                DeliveryMethodId = FixedIds.DeliveryMethod,
                ShipToAddress = new OrderAddressToReturn
                {
                    FirstName = "Ali",
                    LastName = "Hassan",
                    Street = "12 Main St",
                    City = "Cairo",
                    Country = "Egypt"
                }
            },
            UserEmail,
            default);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e.code == "Order.InsufficientStock");
        Assert.Empty(orders);
    }

    private static (OrderServices Services, List<Order> SavedOrders) BuildServices(BasketCustomer basket)
    {
        var orders = new List<Order>();

        var basketRepository = new FakeBasketRepository(basket);
        var unitOfWork = new FakeUnitOfWork(orders);

        // OrderServices only stores the mapper, it never uses it here.
        var services = new OrderServices(basketRepository, unitOfWork, mapper: null!);

        return (services, orders);
    }

    private static class FixedIds
    {
        public static readonly Guid Variant = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid Product = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid Customer = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly Guid DeliveryMethod = Guid.Parse("44444444-4444-4444-4444-444444444444");
    }

    private sealed class FakeBasketRepository(BasketCustomer basket) : IBasketRepository
    {
        public Task<BasketCustomer?> GetBasketAsync(Guid basketId, CancellationToken cancellationToken = default)
            => Task.FromResult<BasketCustomer?>(basketId == basket.Id ? basket : null);

        public Task<BasketCustomer?> CreateOrUpdateBasketAsync(
            BasketCustomer updated,
            TimeSpan? timeToLive = null,
            CancellationToken cancellationToken = default)
            => Task.FromResult<BasketCustomer?>(updated);

        public Task<bool> DeleteBasketAsync(Guid basketId, CancellationToken cancellationToken = default)
            => Task.FromResult(true);
    }

    private sealed class FakeUnitOfWork(List<Order> orders) : IUnitOfWork
    {
        private readonly FakeRepository<Customer> _customers = CreateCustomers();
        private readonly FakeRepository<DeliveryMethod> _deliveryMethods = CreateDeliveryMethods();
        private readonly FakeRepository<ProductVariant> _variants = CreateVariants();

        private static FakeRepository<Customer> CreateCustomers()
        {
            var repository = new FakeRepository<Customer>();
            repository.Add(new Customer
            {
                Id = FixedIds.Customer,
                IdentityUserId = "identity-user-1",
                FullName = "Ali Hassan",
                Email = UserEmail
            });
            return repository;
        }

        private static FakeRepository<DeliveryMethod> CreateDeliveryMethods()
        {
            var repository = new FakeRepository<DeliveryMethod>();
            repository.Add(DeliveryMethod
                .Create(FixedIds.DeliveryMethod, "Standard", 15m, "2-4 business days")
                .Value!);
            return repository;
        }

        private static FakeRepository<ProductVariant> CreateVariants()
        {
            var repository = new FakeRepository<ProductVariant>();
            repository.Add(new ProductVariant
            {
                Id = FixedIds.Variant,
                ProductId = FixedIds.Product,
                Product = new Product
                {
                    Id = FixedIds.Product,
                    Name = "Ethiopian Yirgacheffe",
                    Slug = "ethiopian-yirgacheffe",
                    Description = "Single origin",
                    Origin = "Ethiopia",
                    CategoryId = Guid.NewGuid()
                },
                WeightInGrams = 250,
                Price = 50m,
                StockQuantity = 10,
                Sku = "SKU-001"
            });
            return repository;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct) => Task.FromResult(0);

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            if (typeof(TEntity) == typeof(Customer))
                return (IGenericRepository<TEntity>)(object)_customers;

            if (typeof(TEntity) == typeof(DeliveryMethod))
                return (IGenericRepository<TEntity>)(object)_deliveryMethods;

            if (typeof(TEntity) == typeof(ProductVariant))
                return (IGenericRepository<TEntity>)(object)_variants;

            if (typeof(TEntity) == typeof(Order))
                return (IGenericRepository<TEntity>)(object)new FakeRepository<Order>(orders);

            throw new NotSupportedException($"No fake repository for {typeof(TEntity).Name}.");
        }

        public IProductRepository ProductRepository()
            => throw new NotSupportedException();
    }

    private sealed class FakeRepository<T>(List<T>? storage = null) : IGenericRepository<T>
        where T : BaseEntity
    {
        private readonly List<T> _storage = storage ?? [];

        public void Add(T entity) => _storage.Add(entity);

        public void Update(T entity)
        {
        }

        public void Delete(T entity) => _storage.Remove(entity);

        public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<T>>(_storage.ToList());

        public Task<IReadOnlyList<T>> GetAllAsync(ISpecification<T> specs, CancellationToken ct = default)
        {
            IEnumerable<T> query = _storage;

            if (specs.WhereExpression is not null)
                query = query.Where(specs.WhereExpression.Compile());

            return Task.FromResult<IReadOnlyList<T>>(query.ToList());
        }

        public Task<int> CountAsync(ISpecification<T> specs, CancellationToken ct = default)
            => Task.FromResult(_storage.Count(specs.WhereExpression!.Compile()));
    }
}
