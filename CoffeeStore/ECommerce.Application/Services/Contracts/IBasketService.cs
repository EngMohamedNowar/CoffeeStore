using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Baskets;

namespace ECommerce.Application.Services.Contracts;

public interface IBasketService
{
    Task<Result<BasketDto>> GetBasketAsync(Guid basketId, CancellationToken cancellationToken = default);

    Task<Result<BasketDto>> CreateOrUpdateBasketAsync(BasketDto basket, TimeSpan? timeToLive = null, CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteBasketAsync(Guid basketId, CancellationToken cancellationToken = default);
}
