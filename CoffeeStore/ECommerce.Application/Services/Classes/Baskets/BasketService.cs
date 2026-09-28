using AutoMapper;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Baskets;
using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Baskets;

namespace ECommerce.Application.Services.Classes.Baskets;

public class BasketService(IBasketRepository repository, IMapper mapper) : IBasketService
{
    public async Task<Result<BasketDto>> CreateOrUpdateBasketAsync(
        BasketDto basket,
        TimeSpan? timeToLive = null,
        CancellationToken cancellationToken = default)
    {
        var basketCustomer = mapper.Map<BasketCustomer>(basket);

        var result = await repository.CreateOrUpdateBasketAsync(basketCustomer, timeToLive, cancellationToken);

        if (result is null)
        {
            return Result<BasketDto>.Fail(Error.Failure(
                "Basket.CreateOrUpdate.Failure",
                "Failed to create or update basket"));
        }

        return Result<BasketDto>.Ok(mapper.Map<BasketDto>(result));
    }

    public async Task<Result<BasketDto>> GetBasketAsync(Guid basketId, CancellationToken cancellationToken = default)
    {
        var basket = await repository.GetBasketAsync(basketId, cancellationToken);

        if (basket is null)
        {
            return Result<BasketDto>.Fail(Error.NotFound(
                "Basket.GetBasket.NotFound",
                $"Basket with id {basketId} was not found"));
        }

        return Result<BasketDto>.Ok(mapper.Map<BasketDto>(basket));
    }

    public async Task<Result<bool>> DeleteBasketAsync(Guid basketId, CancellationToken cancellationToken = default)
    {
        var basket = await repository.GetBasketAsync(basketId, cancellationToken);

        if (basket is null)
        {
            return Result<bool>.Fail(Error.NotFound(
                "Basket.GetBasket.NotFound",
                $"Basket with id {basketId} was not found"));
        }

        var deleted = await repository.DeleteBasketAsync(basketId, cancellationToken);

        if (!deleted)
        {
            return Result<bool>.Fail(Error.Failure(
                "Basket.DeleteBasket.Failure",
                $"Failed to delete basket {basketId}"));
        }

        return Result<bool>.Ok(true);
    }
}
