namespace ECommerce.Application.Common.Messaging;

/// <summary>
/// Marker for a request (typically a write) that produces <typeparamref name="TResponse"/>.
/// Pair it with an <see cref="IRequestHandler{TRequest,TResponse}"/> to execute it.
/// </summary>
public interface IRequest<TResponse>
{
}
