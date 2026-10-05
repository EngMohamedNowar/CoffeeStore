using ECommerce.Domain.Common;
using ECommerce.Domain.Entities.Customers;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities.Orders;

public class Order : BaseEntity
{
    private Order()
    {
    }

    public string OrderNumber { get; private set; } = string.Empty;

    public Guid CustomerId { get; private set; }
    public Customer? Customer { get; private set; }

    public string UserEmail { get; private set; } = string.Empty;

    public Guid ShippingAddressId { get; private set; }
    public OrderAddress ShipToAddress { get; private set; } = null!;

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public decimal SubTotal { get; private set; }
    public decimal ShippingFee { get; private set; }
    public decimal DeliveryMethodPrice { get; private set; }

    public Guid DeliveryMethodId { get; private set; }
    public DeliveryMethod? DeliveryMethod { get; private set; }

    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
    public Payment? Payment { get; private set; }

    public decimal GetTotal() => SubTotal + DeliveryMethodPrice + ShippingFee;

    public static Result<Order> Create(
        Guid id,
        Guid customerId,
        string userEmail,
        OrderAddress shipToAddress,
        DeliveryMethod deliveryMethod,
        IEnumerable<OrderItem> items,
        decimal shippingFee = 0m,
        Guid shippingAddressId = default)
    {
        var basket = items?.ToList() ?? new List<OrderItem>();

        var invalid = Validate(
            id,
            customerId,
            userEmail,
            shipToAddress,
            deliveryMethod,
            basket,
            shippingFee);

        if (invalid is not null)
            return Result<Order>.Failure(invalid);

        var order = new Order
        {
            Id = id,
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            UserEmail = userEmail.Trim(),
            ShippingAddressId = shippingAddressId,
            ShipToAddress = CopyAddress(shipToAddress),
            DeliveryMethodId = deliveryMethod.Id,
            DeliveryMethodPrice = deliveryMethod.Price,
            ShippingFee = shippingFee,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in basket)
        {
            item.AssignOrder(order.Id);
            order.Items.Add(item);
        }

        order.SubTotal = order.Items.Sum(item => item.LineTotal);

        return Result<Order>.Success(order);
    }

    public Result Cancel()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.CannotCancel);

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result MarkAsPaid()
    {
        if (Status == OrderStatus.Cancelled)
            return Result.Failure(OrderErrors.CannotPayCancelled);

        if (Status is not OrderStatus.Pending and not OrderStatus.Confirmed)
            return Result.Failure(OrderErrors.InvalidPaymentState);

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static Error? Validate(
        Guid id,
        Guid customerId,
        string userEmail,
        OrderAddress shipToAddress,
        DeliveryMethod deliveryMethod,
        IReadOnlyList<OrderItem> items,
        decimal shippingFee)
    {
        if (id == Guid.Empty)
            return OrderErrors.InvalidId;

        if (customerId == Guid.Empty)
            return OrderErrors.CustomerIdRequired;

        if (string.IsNullOrWhiteSpace(userEmail))
            return OrderErrors.InvalidEmail;

        if (shipToAddress is null || IsIncomplete(shipToAddress))
            return OrderErrors.ShippingAddressRequired;

        if (deliveryMethod is null)
            return OrderErrors.DeliveryMethodRequired;

        if (!deliveryMethod.IsAvailable)
            return OrderErrors.DeliveryMethodUnavailable;

        if (items.Count == 0)
            return OrderErrors.EmptyBasket;

        if (shippingFee < 0)
            return OrderErrors.InvalidShippingFee;

        return null;
    }

    private static bool IsIncomplete(OrderAddress address)
        => string.IsNullOrWhiteSpace(address.FirstName)
            || string.IsNullOrWhiteSpace(address.LastName)
            || string.IsNullOrWhiteSpace(address.Street)
            || string.IsNullOrWhiteSpace(address.City)
            || string.IsNullOrWhiteSpace(address.Country);

    private static OrderAddress CopyAddress(OrderAddress address)
        => new()
        {
            FirstName = address.FirstName.Trim(),
            LastName = address.LastName.Trim(),
            Street = address.Street.Trim(),
            City = address.City.Trim(),
            Country = address.Country.Trim()
        };

    private static string GenerateOrderNumber()
        => $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
}
