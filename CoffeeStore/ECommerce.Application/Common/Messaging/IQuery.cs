namespace ECommerce.Application.Common.Messaging;

/// <summary>
/// Marker for a read that produces <typeparamref name="TResponse"/>.
/// Pair it with an <see cref="IQueryHandler{TQuery,TResponse}"/> to execute it.
/// </summary>
public interface IQuery<TResponse>
{
}
