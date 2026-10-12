using ECommerce.Domain.Contracts.Repositories;
using StackExchange.Redis;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class CacheRepository(IConnectionMultiplexer connection) : ICacheRepository
{
    private readonly IDatabase _dataBase = connection.GetDatabase();

    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _dataBase.StringGetAsync(key);
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task SetAsync(string key, string value, TimeSpan? duration = null, CancellationToken cancellationToken = default)
    {
        await _dataBase.StringSetAsync(key, value, duration ?? TimeSpan.FromMinutes(1));
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _dataBase.KeyDeleteAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        foreach (var endpoint in connection.GetEndPoints())
        {
            var server = connection.GetServer(endpoint);
            if (!server.IsConnected || server.IsReplica)
                continue;

            foreach (var key in server.Keys(pattern: prefix + "*"))
            {
                await _dataBase.KeyDeleteAsync(key);
            }
        }
    }
}
