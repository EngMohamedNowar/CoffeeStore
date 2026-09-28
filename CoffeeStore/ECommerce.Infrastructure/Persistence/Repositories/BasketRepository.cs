using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Baskets;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class BasketRepository(IConnectionMultiplexer connection) : IBasketRepository
{
    private const string KeyPrefix = "basket:";
    private readonly IDatabase _dataBase = connection.GetDatabase();

    public async Task<BasketCustomer?> GetBasketAsync(Guid basketId, CancellationToken cancellationToken = default)
    {
        var value = await _dataBase.StringGetAsync(BuildKey(basketId));
        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<BasketCustomer>(value.ToString());
    }

    public async Task<BasketCustomer?> CreateOrUpdateBasketAsync(
        BasketCustomer basket,
        TimeSpan? timeToLive = null,
        CancellationToken cancellationToken = default)
    {
        var serializedBasket = JsonSerializer.SerializeToUtf8Bytes(basket);

        var isSet = await _dataBase.StringSetAsync(
            BuildKey(basket.Id),
            serializedBasket,
            timeToLive ?? TimeSpan.FromDays(7));

        return isSet ? basket : null;
    }

    public async Task<bool> DeleteBasketAsync(Guid basketId, CancellationToken cancellationToken = default)
        => await _dataBase.KeyDeleteAsync(BuildKey(basketId));

    private static string BuildKey(Guid basketId) => KeyPrefix + basketId;
}
