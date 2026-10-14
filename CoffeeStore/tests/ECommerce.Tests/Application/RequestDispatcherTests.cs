using ECommerce.Application.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Tests.Application;

public class RequestDispatcherTests
{
    private sealed record TestRequest(string Name) : IRequest<string>;

    private sealed class TestRequestHandler : IRequestHandler<TestRequest, string>
    {
        public Task<string> HandleAsync(TestRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult($"handled:{request.Name}");
    }

    private sealed class TokenRecordingHandler : IRequestHandler<TestRequest, string>
    {
        public CancellationToken LastToken { get; private set; }

        public Task<string> HandleAsync(TestRequest request, CancellationToken cancellationToken = default)
        {
            LastToken = cancellationToken;
            return Task.FromResult("ok");
        }
    }

    [Fact]
    public async Task SendAsync_ResolvesTheRegisteredHandler_AndReturnsItsResponse()
    {
        var services = new ServiceCollection();
        services.AddScoped<IRequestDispatcher, RequestDispatcher>();
        services.AddScoped<IRequestHandler<TestRequest, string>, TestRequestHandler>();

        using var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IRequestDispatcher>();

        var response = await dispatcher.SendAsync(new TestRequest("coffee"));

        Assert.Equal("handled:coffee", response);
    }

    [Fact]
    public async Task SendAsync_ForwardsTheCancellationTokenToTheHandler()
    {
        var services = new ServiceCollection();
        services.AddScoped<IRequestDispatcher, RequestDispatcher>();
        services.AddScoped<IRequestHandler<TestRequest, string>, TokenRecordingHandler>();

        using var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IRequestDispatcher>();
        var handler = (TokenRecordingHandler)scope.ServiceProvider
            .GetRequiredService<IRequestHandler<TestRequest, string>>();

        using var cts = new CancellationTokenSource();
        await dispatcher.SendAsync(new TestRequest("coffee"), cts.Token);

        Assert.Equal(cts.Token, handler.LastToken);
    }

    [Fact]
    public async Task SendAsync_FailsLoudly_WhenNoHandlerIsRegistered()
    {
        var services = new ServiceCollection();
        services.AddScoped<IRequestDispatcher, RequestDispatcher>();

        using var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IRequestDispatcher>();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.SendAsync(new TestRequest("coffee")));
    }

    [Fact]
    public async Task SendAsync_RejectsANullRequest()
    {
        var services = new ServiceCollection();
        services.AddScoped<IRequestDispatcher, RequestDispatcher>();

        using var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IRequestDispatcher>();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => dispatcher.SendAsync<string>(null!));
    }

    [Fact]
    public async Task SendAsync_IsUsableAcrossRepeatedCalls_WithAScopedHandler()
    {
        var services = new ServiceCollection();
        services.AddScoped<IRequestDispatcher, RequestDispatcher>();
        services.AddScoped<IRequestHandler<TestRequest, string>, TestRequestHandler>();

        using var provider = services.BuildServiceProvider();

        string first;
        string second;

        using (var scope = provider.CreateScope())
        {
            var dispatcher = scope.ServiceProvider.GetRequiredService<IRequestDispatcher>();
            first = await dispatcher.SendAsync(new TestRequest("a"));
        }

        using (var scope = provider.CreateScope())
        {
            var dispatcher = scope.ServiceProvider.GetRequiredService<IRequestDispatcher>();
            second = await dispatcher.SendAsync(new TestRequest("b"));
        }

        Assert.Equal("handled:a", first);
        Assert.Equal("handled:b", second);
    }
}
