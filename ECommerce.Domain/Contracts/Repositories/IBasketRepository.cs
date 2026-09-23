using ECommerce.Domain.Entities.Baskets;

namespace ECommerce.Domain.Contracts.Repositories
{
    public interface IBasketRepository
    {
        Task<BasketCustomer?> GetBasketAsync(Guid basketId, CancellationToken cancellationToken = default);
        Task<BasketCustomer?> CreateOrUpdateBasketAsync(BasketCustomer basket, TimeSpan? timeToLive = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteBasketAsync(Guid basketId, CancellationToken cancellationToken = default);
    }
}