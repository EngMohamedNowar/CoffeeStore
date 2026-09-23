using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Contracts.Repositories;

namespace ECommerce.Application.Services.Classes.Cache
{
    public class CacheServices(ICacheRepository repository) : ICacheServices
    {
        public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            return await repository.GetAsync(key, cancellationToken);
        }

        public async Task SetAsync(string key, object value, TimeSpan? duration = null, CancellationToken cancellationToken = default)
        {
            await repository.SetAsync(key, value, duration ?? TimeSpan.FromMinutes(1), cancellationToken);
        }
    }
}