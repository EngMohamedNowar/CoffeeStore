namespace ECommerce.Application.Common.Messaging;

/// <summary>
/// Resolves the handler registered for a request and runs it.
/// Controllers depend on this instead of on concrete handlers.
/// </summary>
public interface IRequestDispatcher
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
