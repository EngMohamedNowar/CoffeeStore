using ECommerce.Domain.Entities.Customers;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Tests.Infrastructure;

public class OrderPersistenceTests
{
    private static StoreDbContext BuildStoreContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new StoreDbContext(options);
    }

    private static OrderItem Item(decimal unitPrice = 50m, int quantity = 2)
        => OrderItem.Create(
            Guid.NewGuid(),
            new ProductItemOrdered(Guid.NewGuid(), "Ethiopian Yirgacheffe", 250, unitPrice),
            quantity).Value!;

    [Fact]
    public void Order_SavesAndReloadsThroughThePrivateSetters()
    {
        var databaseName = Guid.NewGuid().ToString();
        using var context = BuildStoreContext(databaseName);

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            IdentityUserId = "identity-user-1",
            FullName = "Ali Hassan",
            Email = "ali@example.com"
        };
        context.Customers.Add(customer);

        var deliveryMethod = DeliveryMethod
            .Create(Guid.NewGuid(), "Standard", 15m, "2-4 business days")
            .Value!;
        context.DeliveryMethods.Add(deliveryMethod);

        var order = Order.Create(
            Guid.NewGuid(),
            customer.Id,
            customer.Email,
            new OrderAddress
            {
                FirstName = "Ali",
                LastName = "Hassan",
                Street = "12 Main St",
                City = "Cairo",
                Country = "Egypt"
            },
            deliveryMethod,
            new[] { Item() },
            shippingFee: 5m).Value!;

        context.Orders.Add(order);
        context.SaveChanges();

        using var fresh = BuildStoreContext(databaseName);
        var loaded = fresh.Orders
            .Include(o => o.Customer)
            .Include(o => o.DeliveryMethod)
            .Include(o => o.Items)
            .Single();

        Assert.Equal(order.Id, loaded.Id);
        Assert.Equal(order.OrderNumber, loaded.OrderNumber);
        Assert.Equal("ali@example.com", loaded.UserEmail);
        Assert.Equal(OrderStatus.Pending, loaded.Status);
        Assert.Equal(100m, loaded.SubTotal);
        Assert.Equal(15m, loaded.DeliveryMethodPrice);
        Assert.Equal(5m, loaded.ShippingFee);
        Assert.Equal(120m, loaded.GetTotal());
        Assert.Equal(deliveryMethod.Id, loaded.DeliveryMethodId);

        Assert.Equal("Ali", loaded.ShipToAddress.FirstName);
        Assert.Equal("Hassan", loaded.ShipToAddress.LastName);
        Assert.Equal("12 Main St", loaded.ShipToAddress.Street);
        Assert.Equal("Cairo", loaded.ShipToAddress.City);
        Assert.Equal("Egypt", loaded.ShipToAddress.Country);

        var item = Assert.Single(loaded.Items);
        Assert.Equal(loaded.Id, item.OrderId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(50m, item.ItemOrdered.UnitPrice);
        Assert.Equal("Ethiopian Yirgacheffe", item.ItemOrdered.ProductName);

        Assert.NotNull(loaded.Customer);
        Assert.Equal(customer.Id, loaded.Customer!.Id);
        Assert.NotNull(loaded.DeliveryMethod);
        Assert.Equal("Standard", loaded.DeliveryMethod!.Name);
    }

    [Fact]
    public void CancelledOrder_PersistsTheStatusChange()
    {
        var databaseName = Guid.NewGuid().ToString();
        using var context = BuildStoreContext(databaseName);

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            IdentityUserId = "identity-user-2",
            FullName = "Sara Adel",
            Email = "sara@example.com"
        };
        context.Customers.Add(customer);

        var deliveryMethod = DeliveryMethod
            .Create(Guid.NewGuid(), "Express", 30m, "next day")
            .Value!;
        context.DeliveryMethods.Add(deliveryMethod);

        var order = Order.Create(
            Guid.NewGuid(),
            customer.Id,
            customer.Email,
            new OrderAddress
            {
                FirstName = "Sara",
                LastName = "Adel",
                Street = "5 Nile St",
                City = "Giza",
                Country = "Egypt"
            },
            deliveryMethod,
            new[] { Item() }).Value!;

        context.Orders.Add(order);
        context.SaveChanges();

        order.Cancel();
        context.SaveChanges();

        using var fresh = BuildStoreContext(databaseName);
        var loaded = fresh.Orders.Single();

        Assert.Equal(OrderStatus.Cancelled, loaded.Status);
        Assert.NotNull(loaded.UpdatedAt);
    }
}
