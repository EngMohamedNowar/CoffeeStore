using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Orders.Dtos;

public class OrderMapping
{
    public static OrderResponse ToResponse(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        return new OrderResponse(
            order.Id,
            order.CustomerId,
            (int)order.Status,
            order.DeliveryMethodId,
            order.DeliveryMethod?.Name ?? string.Empty,
            order.DeliveryMethod?.EstimatedDeliveryTime ?? string.Empty,
            new OrderShippingAddressResponse(
                order.ShipToAddress.FirstName,
                order.ShipToAddress.LastName,
                order.ShipToAddress.Street,
                order.ShipToAddress.City,
                order.ShipToAddress.Country),
            order.SubTotal,
            order.ShippingFee,
            order.GetTotal(),
            new DateTimeOffset(DateTime.SpecifyKind(order.CreatedAt, DateTimeKind.Utc)),
            order.Items
                .Select(item => new OrderItemResponse(
                    item.Id,
                    new ProductItemOrderedResponse(
                        item.ItemOrdered.ProductVariantId,
                        item.ItemOrdered.ProductName,
                        item.ItemOrdered.WeightInGrams,
                        item.ItemOrdered.UnitPrice),
                    item.Quantity,
                    item.LineTotal))
                .ToList());
    }
}
