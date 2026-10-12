namespace ECommerce.Application.Common.Messaging;

/// <summary>
/// Resolves the handler registered for a query and runs it.
/// Controllers depend on this instead of on concrete handlers.
/// </summary>
public interface IQueryDispatcher
{
    Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
