namespace ECommerce.Application.Common.Messaging;

/// <summary>
/// Executes a <typeparamref name="TQuery"/> and produces its response.
/// Register one concrete handler per query in DI.
/// </summary>
public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
