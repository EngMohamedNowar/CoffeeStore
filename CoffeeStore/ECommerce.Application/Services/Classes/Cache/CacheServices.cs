using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Contracts.Repositories;

namespace ECommerce.Application.Services.Classes.Cache
{
    public class CacheServices(ICacheRepository repository) : ICacheServices
    {
        public Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            return repository.GetAsync(key, cancellationToken);
        }

        public Task SetAsync(string key, string value, TimeSpan? duration = null, CancellationToken cancellationToken = default)
        {
            return repository.SetAsync(key, value, duration ?? TimeSpan.FromMinutes(1), cancellationToken);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return repository.RemoveAsync(key, cancellationToken);
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            return repository.RemoveByPrefixAsync(prefix, cancellationToken);
        }
    }
}
