using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Tests.Domain;

public class OrderChangeStatusTests
{
    private static Order NewOrder()
    {
        var delivery = DeliveryMethod.Create(
            Guid.NewGuid(),
            "Standard",
            15m,
            "2-4 business days",
            isAvailable: true).Value!;

        var address = OrderAddress.Create("Ali", "Hassan", "12 Main St", "Cairo", "Egypt").Value!;

        var item = OrderItem.Create(
            Guid.NewGuid(),
            ProductItemOrdered.Create(Guid.NewGuid(), "Ethiopian Yirgacheffe", 250, 50m).Value!,
            1).Value!;

        return Order.Create(Guid.NewGuid(), Guid.NewGuid(), "ali@example.com", address, delivery, new[] { item }).Value!;
    }

    private static Order AtStatus(OrderStatus status)
    {
        var order = NewOrder();

        var path = status switch
        {
            OrderStatus.Pending => Array.Empty<OrderStatus>(),
            OrderStatus.Confirmed => [OrderStatus.Confirmed],
            OrderStatus.Processing => [OrderStatus.Confirmed, OrderStatus.Processing],
            OrderStatus.Shipped => [OrderStatus.Confirmed, OrderStatus.Processing, OrderStatus.Shipped],
            OrderStatus.Delivered => [OrderStatus.Confirmed, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered],
            OrderStatus.Cancelled => [OrderStatus.Cancelled],
            OrderStatus.Refunded => [OrderStatus.Confirmed, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered, OrderStatus.Refunded],
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };

        foreach (var next in path)
        {
            var step = order.ChangeStatus(next);
            Assert.True(step.IsSuccess, string.Join(", ", step.Errors.Select(e => e.code)));
        }

        return order;
    }

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Cancelled)]
    public void ChangeStatus_FromPending_AllowsConfirmedOrCancelled(OrderStatus next)
    {
        var order = NewOrder();

        var result = order.ChangeStatus(next);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(next, order.Status);
        Assert.NotNull(order.UpdatedAt);
    }

    [Fact]
    public void ChangeStatus_ConfirmedToProcessing_Succeeds()
    {
        var order = AtStatus(OrderStatus.Confirmed);

        var result = order.ChangeStatus(OrderStatus.Processing);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(OrderStatus.Processing, order.Status);
    }

    [Fact]
    public void ChangeStatus_ProcessingToShipped_Succeeds()
    {
        var order = AtStatus(OrderStatus.Processing);

        var result = order.ChangeStatus(OrderStatus.Shipped);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Fact]
    public void ChangeStatus_ShippedToDelivered_Succeeds()
    {
        var order = AtStatus(OrderStatus.Shipped);

        var result = order.ChangeStatus(OrderStatus.Delivered);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public void ChangeStatus_DeliveredToRefunded_Succeeds()
    {
        var order = AtStatus(OrderStatus.Delivered);

        var result = order.ChangeStatus(OrderStatus.Refunded);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(OrderStatus.Refunded, order.Status);
    }

    [Fact]
    public void ChangeStatus_SkippingStates_Fails()
    {
        var order = NewOrder();

        var result = order.ChangeStatus(OrderStatus.Delivered);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidStatusTransition", Assert.Single(result.Errors).code);
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void ChangeStatus_ToTheSameStatus_Fails()
    {
        var order = AtStatus(OrderStatus.Confirmed);

        var result = order.ChangeStatus(OrderStatus.Confirmed);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidStatusTransition", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void ChangeStatus_FromCancelled_Fails()
    {
        var order = AtStatus(OrderStatus.Cancelled);

        var result = order.ChangeStatus(OrderStatus.Confirmed);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidStatusTransition", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void ChangeStatus_FromRefunded_Fails()
    {
        var order = AtStatus(OrderStatus.Refunded);

        var result = order.ChangeStatus(OrderStatus.Delivered);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidStatusTransition", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void ChangeStatus_WithUndefinedEnumValue_Fails()
    {
        var order = NewOrder();

        var result = order.ChangeStatus((OrderStatus)999);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidStatusTransition", Assert.Single(result.Errors).code);
    }
}
