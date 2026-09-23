using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Baskets;
using ECommerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services.Contracts
{
    public interface IBasketService
    {

        Task<Result<BasketDto?>> GetBasketAsync(Guid basketId, CancellationToken cancellationToken = default);
        Task<Result<BasketDto?>> CreateOrUpdateBasketAsync(BasketDto basket, TimeSpan? timeToLive = default, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteBasketAsync(Guid basketId, CancellationToken cancellationToken = default);
    }
}
