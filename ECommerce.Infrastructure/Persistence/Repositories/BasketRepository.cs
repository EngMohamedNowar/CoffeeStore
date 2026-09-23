using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Baskets;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class BasketRepository(IConnectionMultiplexer connection) : IBasketRepository
    {
        private readonly IDatabase _dataBase = connection.GetDatabase();

        public async Task<BasketCustomer?> GetBasketAsync(Guid basketId, CancellationToken cancellationToken = default)
        {
            var value = await _dataBase.StringGetAsync(basketId.ToString());
            if (value.IsNullOrEmpty) return null;
            return JsonSerializer.Deserialize<BasketCustomer>(value.ToString());
        }

        public async Task<BasketCustomer?> CreateOrUpdateBasketAsync(BasketCustomer basket, TimeSpan? timeToLive = null, CancellationToken cancellationToken = default)
        {
            var serializedBasket = JsonSerializer.SerializeToUtf8Bytes(basket);

            var isSet = await _dataBase.StringSetAsync(
                basket.Id.ToString(),
                serializedBasket,
                timeToLive ?? TimeSpan.FromDays(7));

            return isSet ? basket : null;
        }

        public async Task<bool> DeleteBasketAsync(Guid basketId, CancellationToken cancellationToken = default)
        {
            return await _dataBase.KeyDeleteAsync(basketId.ToString());
        }
    }
}