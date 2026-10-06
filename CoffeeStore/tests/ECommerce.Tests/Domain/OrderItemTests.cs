using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Errors;

namespace ECommerce.Tests.Domain;

public class OrderItemTests
{
    private static ProductItemOrdered Item(
        decimal unitPrice = 50m,
        int weightInGrams = 250)
        => new(Guid.NewGuid(), "Ethiopian Yirgacheffe", weightInGrams, unitPrice);

    [Fact]
    public void Create_BuildsAnItemWithTheLineTotal()
    {
        var id = Guid.NewGuid();
        var item = Item(unitPrice: 50m);

        var result = OrderItem.Create(id, item, 3);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(id, result.Value!.Id);
        Assert.Equal(3, result.Value.Quantity);
        Assert.Equal(item, result.Value.ItemOrdered);
        Assert.Equal(150m, result.Value.LineTotal);
        Assert.Equal(Guid.Empty, result.Value.OrderId);
    }

    [Fact]
    public void Create_RejectsAnEmptyId()
    {
        var result = OrderItem.Create(Guid.Empty, Item(), 1);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidItemId", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void Create_RejectsAMissingProduct()
    {
        var result = OrderItem.Create(Guid.NewGuid(), null!, 1);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidProductId", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void Create_RejectsAQuantityBelowOne()
    {
        var result = OrderItem.Create(Guid.NewGuid(), Item(), 0);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidQuantity", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void AssignOrder_SetsTheOwningOrder()
    {
        var result = OrderItem.Create(Guid.NewGuid(), Item(), 1);
        var orderId = Guid.NewGuid();

        result.Value!.AssignOrder(orderId);

        Assert.Equal(orderId, result.Value.OrderId);
    }

    [Fact]
    public void ProductItemOrdered_IsAValueObject()
    {
        var left = new ProductItemOrdered(Guid.NewGuid(), "Ethiopian Yirgacheffe", 250, 50m);
        var right = new ProductItemOrdered(left.ProductVariantId, "Ethiopian Yirgacheffe", 250, 50m);
        var different = new ProductItemOrdered(left.ProductVariantId, "Ethiopian Yirgacheffe", 250, 51m);

        Assert.Equal(left, right);
        Assert.NotEqual(left, different);
    }
}
