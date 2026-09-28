namespace ECommerce.Application.Services.Contracts
{
    public interface ICacheServices
    {
        Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);
        Task SetAsync(string key, object value, TimeSpan? duration = null, CancellationToken cancellationToken = default);
    }
}