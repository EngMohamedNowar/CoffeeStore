using AutoMapper;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Baskets;
using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Baskets;

namespace ECommerce.Application.Services.Classes.Baskets
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _repository;
        private readonly IMapper _mapper;

        public BasketService(
            IBasketRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<BasketDto?>> CreateOrUpdateBasketAsync(
            BasketDto basket,
            TimeSpan? timeToLive = null,
            CancellationToken cancellationToken = default)
        {
            var basketCustomer =
                _mapper.Map<BasketCustomer>(basket);

            var result =
                await _repository.CreateOrUpdateBasketAsync(
                    basketCustomer,
                    timeToLive,
                    cancellationToken);

            if (result is null)
            {
                return Result<BasketDto>.Fail(
                    Error.Failure(
                        "Basket.CreateOrUpdate.Failure",
                        "Failed to create or update basket"));
            }

            var basketDto =
                _mapper.Map<BasketDto>(result);

            return Result<BasketDto>.Ok(basketDto);
        }

        public async Task<Result<BasketDto?>> GetBasketAsync(
            Guid basketId,
            CancellationToken cancellationToken = default)
        {
            var basket =
                await _repository.GetBasketAsync(
                    basketId,
                    cancellationToken);

            if (basket is null)
            {
                return Result<BasketDto>.Fail(
                    Error.NotFound(
                        "Basket.GetBasket.NotFound",
                        $"Basket with id {basketId} was not found"));
            }

            var basketDto =
                _mapper.Map<BasketDto>(basket);

            return Result<BasketDto>.Ok(basketDto);
        }

        public async Task<Result<bool>> DeleteBasketAsync(
            Guid basketId,
            CancellationToken cancellationToken = default)
        {
            var basket =
                await _repository.GetBasketAsync(
                    basketId,
                    cancellationToken);

            if (basket is null)
            {
                return Result<bool>.Fail(
                    Error.NotFound(
                        "Basket.GetBasket.NotFound",
                        $"Basket with id {basketId} was not found"));
            }

            var result =
                await _repository.DeleteBasketAsync(
                    basketId,
                    cancellationToken);

            if (!result)
            {
                return Result<bool>.Fail(
                    Error.Failure(
                        "Basket.DeleteBasket.Failure",
                        $"Failed to delete basket {basketId}"));
            }

            return Result<bool>.Ok(true);
        }
    }
}
