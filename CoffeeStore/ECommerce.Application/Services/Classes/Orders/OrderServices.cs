using AutoMapper;
using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.DeliveryMethods.Specification;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Services.Contracts;
using ECommerce.Application.Specifications;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Customers;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Products;
using OrderItem = ECommerce.Domain.Entities.Orders.OrderItem;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Application.Services.Classes.Orders;

public class OrderServices(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IMapper mapper) : IOrderServices
{
    public async Task<Result<OrderToReturn>> CreateOrderAsync(OrderDto dto, string email, CancellationToken ct)
    {
        var basket = await basketRepository.GetBasketAsync(dto.BasketId, ct);
        if (basket is null)
        {
            return Result<OrderToReturn>.Fail(Error.NotFound("Order.BasketNotFound", $"Basket with id {dto.BasketId} was not found"));
        }

        var deliveryMethods = await unitOfWork.GetRepository<DeliveryMethod>()
            .GetAllAsync(new DeliveryMethodByIdSpecification(dto.DeliveryMethodId), ct);

        var deliveryMethod = deliveryMethods.FirstOrDefault();
        if (deliveryMethod is null)
        {
            return Result<OrderToReturn>.Fail(DeliveryMethodErrors.NotFound);
        }

        var customers = await unitOfWork.GetRepository<Customer>()
            .GetAllAsync(new CustomerByEmailSpecification(email), ct);

        var customer = customers.FirstOrDefault();
        if (customer is null)
        {
            return Result<OrderToReturn>.Fail(Error.NotFound("Order.CustomerNotFound", $"Customer with email {email} was not found"));
        }

        var variantIds = basket.Items.Select(item => item.ProductVariantId).Distinct().ToList();
        var variants = await unitOfWork.GetRepository<ProductVariant>()
            .GetAllAsync(new ProductVariantsByIdsSpecification(variantIds), ct);

        var variantsById = variants.ToDictionary(v => v.Id);

        var orderItems = new List<OrderItem>();
        foreach (var item in basket.Items)
        {
            if (!variantsById.TryGetValue(item.ProductVariantId, out var variant))
            {
                return Result<OrderToReturn>.Fail(Error.NotFound(
                    "Order.ProductVariantNotFound",
                    $"Product variant with id {item.ProductVariantId} was not found"));
            }

            if (item.Quantity > variant.StockQuantity)
            {
                return Result<OrderToReturn>.Fail(OrderErrors.InsufficientStock);
            }

            var itemOrderedResult = ProductItemOrdered.Create(
                variant.Id,
                variant.Product.Name,
                variant.WeightInGrams,
                variant.Price);

            if (itemOrderedResult.IsFailure)
            {
                return Result<OrderToReturn>.Fail(itemOrderedResult.Errors);
            }

            var orderItemResult = OrderItem.Create(
                Guid.NewGuid(),
                itemOrderedResult.Value!,
                item.Quantity);

            if (orderItemResult.IsFailure)
            {
                return Result<OrderToReturn>.Fail(orderItemResult.Errors);
            }

            orderItems.Add(orderItemResult.Value!);
        }

        var addressResult = OrderAddress.Create(
            dto.ShipToAddress.FirstName,
            dto.ShipToAddress.LastName,
            dto.ShipToAddress.Street,
            dto.ShipToAddress.City,
            dto.ShipToAddress.Country);

        if (addressResult.IsFailure)
        {
            return Result<OrderToReturn>.Fail(addressResult.Errors);
        }

        var orderResult = Order.Create(
            Guid.NewGuid(),
            customer.Id,
            email,
            addressResult.Value!,
            deliveryMethod,
            orderItems);

        if (orderResult.IsFailure)
        {
            return Result<OrderToReturn>.Fail(orderResult.Errors);
        }

        unitOfWork.GetRepository<Order>().Add(orderResult.Value!);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<OrderToReturn>.Ok(OrderToReturn.FromOrder(orderResult.Value!));
    }
}
