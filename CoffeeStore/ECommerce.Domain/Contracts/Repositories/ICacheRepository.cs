namespace ECommerce.Domain.Contracts.Repositories
{
    public interface ICacheRepository
    {
        Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);
        Task SetAsync(string key, string value, TimeSpan? duration = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    }
}
