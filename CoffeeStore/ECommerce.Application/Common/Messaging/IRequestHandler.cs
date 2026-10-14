namespace ECommerce.Application.Common.Messaging;

/// <summary>
/// Executes a <typeparamref name="TRequest"/> and produces its response.
/// Register one concrete handler per request in DI.
/// </summary>
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
