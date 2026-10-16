using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Specification;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Specifications;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Customers;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Errors;

namespace ECommerce.Application.Orders.Commands.CreateOrder;

internal class CreateOrderCommandHandler(
    IBasketRepository basketRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var basket = await basketRepository.GetBasketAsync(command.BasketId, cancellationToken);
        if (basket is null)
        {
            return Result<OrderResponse>.Fail(Error.NotFound(
                "Order.BasketNotFound",
                $"Basket with id {command.BasketId} was not found"));
        }

        var deliveryMethods = await unitOfWork.GetRepository<DeliveryMethod>()
            .GetAllAsync(new DeliveryMethodByIdSpecification(command.DeliveryMethodId), cancellationToken);

        var deliveryMethod = deliveryMethods.FirstOrDefault();
        if (deliveryMethod is null)
        {
            return Result<OrderResponse>.Fail(DeliveryMethodErrors.NotFound);
        }

        var customers = await unitOfWork.GetRepository<Customer>()
            .GetAllAsync(new CustomerByEmailSpecification(command.Email), cancellationToken);

        var customer = customers.FirstOrDefault();
        if (customer is null)
        {
            return Result<OrderResponse>.Fail(Error.NotFound(
                "Order.CustomerNotFound",
                $"Customer with email {command.Email} was not found"));
        }

        var variantIds = basket.Items.Select(item => item.ProductVariantId).Distinct().ToList();
        var variants = await unitOfWork.GetRepository<ProductVariant>()
            .GetAllAsync(new ProductVariantsByIdsSpecification(variantIds), cancellationToken);

        var variantsById = variants.ToDictionary(v => v.Id);

        var orderItems = new List<OrderItem>();
        foreach (var item in basket.Items)
        {
            if (!variantsById.TryGetValue(item.ProductVariantId, out var variant))
            {
                return Result<OrderResponse>.Fail(Error.NotFound(
                    "Order.ProductVariantNotFound",
                    $"Product variant with id {item.ProductVariantId} was not found"));
            }

            if (item.Quantity > variant.StockQuantity)
            {
                return Result<OrderResponse>.Fail(OrderErrors.InsufficientStock);
            }

            var itemOrderedResult = ProductItemOrdered.Create(
                variant.Id,
                variant.Product.Name,
                variant.WeightInGrams,
                variant.Price);

            if (itemOrderedResult.IsFailure)
            {
                return Result<OrderResponse>.Fail(itemOrderedResult.Errors);
            }

            var orderItemResult = OrderItem.Create(
                Guid.NewGuid(),
                itemOrderedResult.Value!,
                item.Quantity);

            if (orderItemResult.IsFailure)
            {
                return Result<OrderResponse>.Fail(orderItemResult.Errors);
            }

            orderItems.Add(orderItemResult.Value!);
        }

        var addressResult = OrderAddress.Create(
            command.FirstName,
            command.LastName,
            command.Street,
            command.City,
            command.Country);

        if (addressResult.IsFailure)
        {
            return Result<OrderResponse>.Fail(addressResult.Errors);
        }

        var orderResult = Order.Create(
            Guid.NewGuid(),
            customer.Id,
            command.Email,
            addressResult.Value!,
            deliveryMethod,
            orderItems);

        if (orderResult.IsFailure)
        {
            return Result<OrderResponse>.Fail(orderResult.Errors);
        }

        unitOfWork.GetRepository<Order>().Add(orderResult.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrderResponse>.Ok(OrderMapping.ToResponse(orderResult.Value!));
    }
}
