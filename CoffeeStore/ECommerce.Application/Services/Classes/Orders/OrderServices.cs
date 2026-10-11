using AutoMapper;
using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Services.Contracts;
using ECommerce.Application.Specifications;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Customers;
using ECommerce.Domain.Entities.Orders;
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

        var orderItems = new List<OrderItem>();
        foreach (var item in basket.Items)
        {
            var orderItemResult = OrderItem.Create(
                Guid.NewGuid(),
                new ProductItemOrdered(item.ProductId, item.ProductName, 0, item.UnitPrice),
                item.Quantity);

            if (orderItemResult.IsSuccess)
            {
                orderItems.Add(orderItemResult.Value!);
            }
        }

        var orderResult = Order.Create(
            Guid.NewGuid(),
            customer.Id,
            email,
            new OrderAddress
            {
                FirstName = dto.ShipToAddress.FirstName,
                LastName = dto.ShipToAddress.LastName,
                Street = dto.ShipToAddress.Street,
                City = dto.ShipToAddress.City,
                Country = dto.ShipToAddress.Country
            },
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
