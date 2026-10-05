using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Customers;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Persistence.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StoreAddress = ECommerce.Domain.Entities.Addresses.Address;
using IdentityAddress = ECommerce.Domain.Entities.Identity.Address;
using ApplicationUser = ECommerce.Domain.Entities.Identity.ApplicationUser;

namespace ECommerce.Tests.Infrastructure;

public class RelationshipModelTests
{
    private static StoreDbContext BuildStoreContext()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new StoreDbContext(options);
    }

    private static StoreIdentityDbContext BuildIdentityContext()
    {
        var options = new DbContextOptionsBuilder<StoreIdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new StoreIdentityDbContext(options);
    }

    [Fact]
    public void StoreModel_BuildsWithoutThrowing()
    {
        using var context = BuildStoreContext();

        Assert.NotNull(context.Model.FindEntityType(typeof(Customer)));
        Assert.NotNull(context.Model.FindEntityType(typeof(StoreAddress)));
        Assert.NotNull(context.Model.FindEntityType(typeof(Cart)));
        Assert.NotNull(context.Model.FindEntityType(typeof(Order)));
    }

    [Fact]
    public void Address_IsOwnedByACustomerThroughACascadingKey()
    {
        using var context = BuildStoreContext();
        var model = context.Model;

        var customer = model.FindEntityType(typeof(Customer))!;
        var address = model.FindEntityType(typeof(StoreAddress))!;

        // Regression: CustomerId was once removed from the entity while
        // CustomerConfiguration still mapped it, which broke model building.
        Assert.Contains(address.GetProperties(), p => p.Name == nameof(StoreAddress.CustomerId));

        var foreignKey = Assert.Single(address.GetForeignKeys().Where(fk => fk.PrincipalEntityType == customer));
        Assert.Equal(nameof(StoreAddress.CustomerId), foreignKey.Properties.Single().Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);

        Assert.Contains(address.GetNavigations(), n => n.Name == nameof(StoreAddress.Customer));
        Assert.Contains(customer.GetNavigations(), n => n.Name == nameof(Customer.Addresses));
    }

    [Fact]
    public void Cart_BelongsToASingleCustomer()
    {
        using var context = BuildStoreContext();
        var model = context.Model;

        var customer = model.FindEntityType(typeof(Customer))!;
        var cart = model.FindEntityType(typeof(Cart))!;

        var foreignKey = Assert.Single(cart.GetForeignKeys().Where(fk => fk.PrincipalEntityType == customer));
        Assert.Equal(nameof(Cart.CustomerId), foreignKey.Properties.Single().Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);

        Assert.Contains(cart.GetIndexes(), index => index.IsUnique);
        Assert.Contains(cart.GetNavigations(), n => n.Name == nameof(Cart.Customer));
    }

    [Fact]
    public void Order_ReadsFromCustomerAndShippingAddress()
    {
        using var context = BuildStoreContext();
        var model = context.Model;

        var customer = model.FindEntityType(typeof(Customer))!;
        var address = model.FindEntityType(typeof(StoreAddress))!;
        var order = model.FindEntityType(typeof(Order))!;

        var toCustomer = Assert.Single(order.GetForeignKeys().Where(fk => fk.PrincipalEntityType == customer));
        Assert.Equal(DeleteBehavior.Restrict, toCustomer.DeleteBehavior);

        var toAddress = Assert.Single(order.GetForeignKeys().Where(fk => fk.PrincipalEntityType == address));
        Assert.Equal(nameof(Order.ShippingAddressId), toAddress.Properties.Single().Name);
        Assert.Equal(DeleteBehavior.Restrict, toAddress.DeleteBehavior);

        Assert.Contains(order.GetNavigations(), n => n.Name == nameof(Order.Customer));
        Assert.Contains(order.GetNavigations(), n => n.Name == nameof(Order.ShippingAddress));
        Assert.Contains(customer.GetNavigations(), n => n.Name == nameof(Customer.Orders));
    }

    [Fact]
    public void StoreAddressesAreSoftDeletableThroughTheQueryFilter()
    {
        using var context = BuildStoreContext();

        var address = context.Model.FindEntityType(typeof(StoreAddress))!;
        var queryFilter = address.GetQueryFilter();

        Assert.NotNull(queryFilter);
        Assert.Contains("IsDeleted", queryFilter!.ToString());
    }

    [Fact]
    public void IdentityAddress_IsLinkedToTheApplicationUser()
    {
        using var context = BuildIdentityContext();
        var model = context.Model;

        var user = model.FindEntityType(typeof(ApplicationUser))!;
        var address = model.FindEntityType(typeof(IdentityAddress))!;

        Assert.Equal("Addresses", address.GetTableName());

        var foreignKey = Assert.Single(address.GetForeignKeys().Where(fk => fk.PrincipalEntityType == user));
        Assert.Equal(nameof(IdentityAddress.UserId), foreignKey.Properties.Single().Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);

        Assert.Contains(user.GetNavigations(), n => n.Name == nameof(ApplicationUser.Addresses) && n.IsCollection);
        Assert.Contains(address.GetNavigations(), n => n.Name == nameof(IdentityAddress.User));
    }

    [Fact]
    public void IdentityAddress_ExposesTheAddressBookColumns()
    {
        using var context = BuildIdentityContext();

        var address = context.Model.FindEntityType(typeof(IdentityAddress))!;

        foreach (var property in new[]
                 {
                     nameof(IdentityAddress.Label),
                     nameof(IdentityAddress.FirstName),
                     nameof(IdentityAddress.LastName),
                     nameof(IdentityAddress.Country),
                     nameof(IdentityAddress.City),
                     nameof(IdentityAddress.Street),
                     nameof(IdentityAddress.Building),
                     nameof(IdentityAddress.Notes),
                     nameof(IdentityAddress.IsDefault)
                 })
        {
            Assert.Contains(address.GetProperties(), p => p.Name == property);
        }

        Assert.Equal(typeof(Guid), address.FindProperty(nameof(IdentityAddress.Id))!.ClrType);
    }
}
