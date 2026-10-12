using ECommerce.Application.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Tests.Application;

public class QueryDispatcherTests
{
    private sealed record TestQuery(string Name) : IQuery<string>;

    private sealed class TestQueryHandler : IQueryHandler<TestQuery, string>
    {
        public Task<string> HandleAsync(TestQuery query, CancellationToken cancellationToken = default)
            => Task.FromResult($"handled:{query.Name}");
    }

    private sealed class TokenRecordingHandler : IQueryHandler<TestQuery, string>
    {
        public CancellationToken LastToken { get; private set; }

        public Task<string> HandleAsync(TestQuery query, CancellationToken cancellationToken = default)
        {
            LastToken = cancellationToken;
            return Task.FromResult("ok");
        }
    }

    [Fact]
    public async Task SendAsync_ResolvesTheRegisteredHandler_AndReturnsItsResponse()
    {
        var services = new ServiceCollection();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<IQueryHandler<TestQuery, string>, TestQueryHandler>();

        using var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

        var response = await dispatcher.SendAsync(new TestQuery("coffee"));

        Assert.Equal("handled:coffee", response);
    }

    [Fact]
    public async Task SendAsync_ForwardsTheCancellationTokenToTheHandler()
    {
        var services = new ServiceCollection();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<IQueryHandler<TestQuery, string>, TokenRecordingHandler>();

        using var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
        var handler = (TokenRecordingHandler)scope.ServiceProvider
            .GetRequiredService<IQueryHandler<TestQuery, string>>();

        using var cts = new CancellationTokenSource();
        await dispatcher.SendAsync(new TestQuery("coffee"), cts.Token);

        Assert.Equal(cts.Token, handler.LastToken);
    }

    [Fact]
    public async Task SendAsync_FailsLoudly_WhenNoHandlerIsRegistered()
    {
        var services = new ServiceCollection();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        using var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.SendAsync(new TestQuery("coffee")));
    }

    [Fact]
    public async Task SendAsync_RejectsANullQuery()
    {
        var services = new ServiceCollection();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        using var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => dispatcher.SendAsync<string>(null!));
    }

    [Fact]
    public async Task SendAsync_IsUsableAcrossRepeatedCalls_WithAScopedHandler()
    {
        var services = new ServiceCollection();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<IQueryHandler<TestQuery, string>, TestQueryHandler>();

        using var provider = services.BuildServiceProvider();

        string first;
        string second;

        using (var scope = provider.CreateScope())
        {
            var dispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
            first = await dispatcher.SendAsync(new TestQuery("a"));
        }

        using (var scope = provider.CreateScope())
        {
            var dispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
            second = await dispatcher.SendAsync(new TestQuery("b"));
        }

        Assert.Equal("handled:a", first);
        Assert.Equal("handled:b", second);
    }
}
