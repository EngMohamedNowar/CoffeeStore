using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application.Common.Messaging;

/// <summary>
/// Looks up <c>IRequestHandler&lt;TRequest, TResponse&gt;</c> for the runtime type of the
/// request and invokes it. The <see cref="MethodInfo"/> is cached per handler type so
/// the reflection cost is paid once, not on every request.
/// </summary>
public sealed class RequestDispatcher(IServiceProvider serviceProvider) : IRequestDispatcher
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> HandleMethods = new();

    public Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));

        var handler = serviceProvider.GetRequiredService(handlerType);

        var handleMethod = HandleMethods.GetOrAdd(handlerType, static type =>
            type.GetMethod("HandleAsync", BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException(
                $"{type.Name} does not declare a public HandleAsync method."));

        return (Task<TResponse>)handleMethod.Invoke(handler, [request, cancellationToken])!;
    }
}
