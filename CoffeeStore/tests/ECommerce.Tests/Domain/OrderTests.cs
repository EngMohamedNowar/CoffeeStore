using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Tests.Domain;

public class OrderTests
{
    private static DeliveryMethod Delivery(decimal price = 15m, bool available = true)
        => DeliveryMethod.Create(
            Guid.NewGuid(),
            "Standard",
            price,
            "2-4 business days",
            isAvailable: available).Value!;

    private static OrderAddress Address(string city = "Cairo")
        => new()
        {
            FirstName = " Ali ",
            LastName = "Hassan",
            Street = "12 Main St",
            City = city,
            Country = "Egypt"
        };

    private static OrderItem Item(decimal unitPrice = 50m, int quantity = 1)
        => OrderItem.Create(
            Guid.NewGuid(),
            new ProductItemOrdered(Guid.NewGuid(), "Ethiopian Yirgacheffe", 250, unitPrice),
            quantity).Value!;

    private static Result<Order> Create(
        string email = "ali@example.com",
        OrderAddress? address = null,
        DeliveryMethod? deliveryMethod = null,
        IEnumerable<OrderItem>? items = null,
        decimal shippingFee = 0m)
        => Order.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            email,
            address ?? Address(),
            deliveryMethod ?? Delivery(),
            items ?? new[] { Item() },
            shippingFee);

    [Fact]
    public void Create_BuildsAPendingOrderWithASnapshotTotal()
    {
        var delivery = Delivery(price: 15m);

        var result = Create(
            deliveryMethod: delivery,
            items: new[] { Item(unitPrice: 50m, quantity: 2), Item(unitPrice: 30m, quantity: 1) },
            shippingFee: 5m);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));

        var order = result.Value!;
        Assert.Matches(@"^ORD-\d{14}-[0-9A-F]{6}$", order.OrderNumber);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(130m, order.SubTotal);
        Assert.Equal(15m, order.DeliveryMethodPrice);
        Assert.Equal(5m, order.ShippingFee);
        Assert.Equal(150m, order.GetTotal());
        Assert.Equal(delivery.Id, order.DeliveryMethodId);
        Assert.Equal(2, order.Items.Count);
        Assert.Null(order.DeliveryMethod);
    }

    [Fact]
    public void Create_KeepsTheTotalWhenTheDeliveryPriceChangesLater()
    {
        var delivery = Delivery(price: 15m);
        var result = Create(deliveryMethod: delivery);

        delivery.Update("Standard", 999m, "2-4 business days", null, true, 0);

        Assert.Equal(15m, result.Value!.DeliveryMethodPrice);
        Assert.Equal(65m, result.Value.GetTotal());
    }

    [Fact]
    public void Create_CopiesAndTrimsTheAddressSnapshot()
    {
        var address = Address();

        var order = Create(address: address).Value!;
        address.City = "Giza";

        Assert.Equal("Ali", order.ShipToAddress.FirstName);
        Assert.Equal("Cairo", order.ShipToAddress.City);
        Assert.Equal("Egypt", order.ShipToAddress.Country);
    }

    [Fact]
    public void Create_AssignsEveryItemToTheOrder()
    {
        var items = new[] { Item(), Item(), Item() };

        var order = Create(items: items).Value!;

        Assert.Equal(3, order.Items.Count);
        Assert.All(order.Items, item => Assert.Equal(order.Id, item.OrderId));
    }

    [Fact]
    public void Create_GeneratesAUniqueOrderNumber()
    {
        var first = Create().Value!;
        var second = Create().Value!;

        Assert.Matches(@"^ORD-\d{14}-[0-9A-F]{6}$", first.OrderNumber);
        Assert.NotEqual(first.OrderNumber, second.OrderNumber);
    }

    [Fact]
    public void Create_RejectsAnEmptyId()
    {
        var result = Order.Create(Guid.Empty, Guid.NewGuid(), "a@b.com", Address(), Delivery(), new[] { Item() });

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidId", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void Create_RejectsAnEmptyCustomerId()
    {
        var result = Order.Create(Guid.NewGuid(), Guid.Empty, "a@b.com", Address(), Delivery(), new[] { Item() });

        Assert.True(result.IsFailure);
        Assert.Equal("Order.CustomerIdRequired", Assert.Single(result.Errors).code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_RejectsAMissingEmail(string email)
    {
        var result = Create(email: email);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidEmail", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void Create_RejectsAMissingAddress()
    {
        var result = Order.Create(Guid.NewGuid(), Guid.NewGuid(), "a@b.com", null!, Delivery(), new[] { Item() });

        Assert.True(result.IsFailure);
        Assert.Equal("Order.ShippingAddressRequired", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void Create_RejectsAnIncompleteAddress()
    {
        var result = Create(address: Address(city: "  "));

        Assert.True(result.IsFailure);
        Assert.Equal("Order.ShippingAddressRequired", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void Create_RejectsAMissingDeliveryMethod()
    {
        var result = Order.Create(Guid.NewGuid(), Guid.NewGuid(), "a@b.com", Address(), null!, new[] { Item() });

        Assert.True(result.IsFailure);
        Assert.Equal("Order.DeliveryMethodRequired", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void Create_RejectsAnUnavailableDeliveryMethod()
    {
        var result = Create(deliveryMethod: Delivery(available: false));

        Assert.True(result.IsFailure);
        Assert.Equal("Order.DeliveryMethodUnavailable", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void Create_RejectsAnEmptyBasket()
    {
        var result = Create(items: Array.Empty<OrderItem>());

        Assert.True(result.IsFailure);
        Assert.Equal("Order.EmptyBasket", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void Create_RejectsANegativeShippingFee()
    {
        var result = Create(shippingFee: -1m);

        Assert.True(result.IsFailure);
        Assert.Equal("Order.InvalidShippingFee", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void Cancel_FromPending_Succeeds()
    {
        var order = Create().Value!;

        var result = order.Cancel();

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.NotNull(order.UpdatedAt);
    }

    [Fact]
    public void Cancel_FromConfirmed_Fails()
    {
        var order = Create().Value!;
        order.MarkAsPaid();

        var result = order.Cancel();

        Assert.True(result.IsFailure);
        Assert.Equal("Order.CannotCancel", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void MarkAsPaid_FromPending_ConfirmsTheOrder()
    {
        var order = Create().Value!;

        var result = order.MarkAsPaid();

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.NotNull(order.UpdatedAt);
    }

    [Fact]
    public void MarkAsPaid_FromCancelled_Fails()
    {
        var order = Create().Value!;
        order.Cancel();

        var result = order.MarkAsPaid();

        Assert.True(result.IsFailure);
        Assert.Equal("Order.CannotPayCancelled", Assert.Single(result.Errors).code);
    }

    [Fact]
    public void MarkAsPaid_FromConfirmed_SucceedsAgain()
    {
        var order = Create().Value!;
        order.MarkAsPaid();

        var result = order.MarkAsPaid();

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }
}
