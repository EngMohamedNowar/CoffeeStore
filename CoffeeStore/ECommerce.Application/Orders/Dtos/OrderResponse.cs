using System;
using System.Collections.Generic;

namespace ECommerce.Application.Orders.Dtos;

public sealed record ProductItemOrderedResponse(
    Guid ProductVariantId, string ProductName, int WeightInGrams, decimal UnitPrice
    );
public sealed record OrderItemResponse(Guid Id, ProductItemOrderedResponse ItemOrdered,
    int Quantity, decimal LineTotal);

public sealed record OrderShippingAddressResponse(
    string RecipientFirstName,
    string RecipientLastName,
    string Street,
    string City,
    string Country);

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    int Status,
    Guid DeliveryMethodId,
    string DeliveryMethodName,
    string DeliveryMethodEstimatedTime,
    OrderShippingAddressResponse ShippingAddress,
    decimal SubTotal,
    decimal ShippingFee,
    decimal Total,
    DateTimeOffset CreatedAt,
    IReadOnlyList<OrderItemResponse> Items
    );
