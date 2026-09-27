namespace ECommerce.Domain.Contracts.Repositories
{
    public interface ICacheRepository
    {
        Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);
        Task SetAsync(string key, object value, TimeSpan? duration = null, CancellationToken cancellationToken = default);
    }
}