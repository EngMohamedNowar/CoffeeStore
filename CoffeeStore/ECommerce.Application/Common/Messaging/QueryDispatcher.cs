using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application.Common.Messaging;

/// <summary>
/// Looks up <c>IQueryHandler&lt;TQuery, TResponse&gt;</c> for the runtime type of the
/// query and invokes it. The <see cref="MethodInfo"/> is cached per handler type so
/// the reflection cost is paid once, not on every request.
/// </summary>
public sealed class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> HandleMethods = new();

    public Task<TResponse> SendAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));

        var handler = serviceProvider.GetRequiredService(handlerType);

        var handleMethod = HandleMethods.GetOrAdd(handlerType, static type =>
            type.GetMethod("HandleAsync", BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException(
                $"{type.Name} does not declare a public HandleAsync method."));

        return (Task<TResponse>)handleMethod.Invoke(handler, [query, cancellationToken])!;
    }
}
