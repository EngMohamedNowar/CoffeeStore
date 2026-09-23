using ECommerce.Domain.Contracts.Repositories;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class CacheRepository(IConnectionMultiplexer connection) : ICacheRepository
    {
        private readonly IDatabase _dataBase = connection.GetDatabase();

        public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            var value = await _dataBase.StringGetAsync(key);
            if (value.IsNullOrEmpty) return null;
            return value;
        }

        public async Task SetAsync(string key, object value, TimeSpan? duration = null, CancellationToken cancellationToken = default)
        {
            var redisValue = JsonSerializer.Serialize(value);
            await _dataBase.StringSetAsync(key, redisValue, duration ?? TimeSpan.FromMinutes(1));
        }
    }
}