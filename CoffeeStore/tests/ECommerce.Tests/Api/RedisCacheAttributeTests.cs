using System.Security.Claims;
using System.Text.Json;
using ECommerce.Api.Attributes;
using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ECommerce.Tests.Api;

public class RedisCacheAttributeTests
{
    private sealed class FakeCacheServices : ICacheServices
    {
        public Dictionary<string, string> Store { get; } = new(StringComparer.Ordinal);

        public Task<string?> GetAsync(string key, CancellationToken ct = default)
            => Task.FromResult(Store.TryGetValue(key, out var value) ? value : null);

        public Task SetAsync(string key, string value, TimeSpan? duration = null, CancellationToken ct = default)
        {
            Store[key] = value;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken ct = default)
        {
            Store.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
        {
            foreach (var key in Store.Keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList())
                Store.Remove(key);

            return Task.CompletedTask;
        }
    }

    private sealed record FilterRun(IActionResult? Result, bool NextInvoked);

    private static DefaultHttpContext CreateHttpContext(
        string method,
        string path,
        string? query = null,
        string? userId = null)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = method;
        httpContext.Request.Path = path;

        if (!string.IsNullOrEmpty(query))
            httpContext.Request.QueryString = new QueryString(query.StartsWith('?') ? query : "?" + query);

        if (userId is not null)
        {
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId)], "TestAuth"));
        }

        return httpContext;
    }

    private static IServiceProvider CreateServices(FakeCacheServices cache)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ICacheServices>(cache);
        services.AddSingleton(Options.Create(new Microsoft.AspNetCore.Mvc.JsonOptions()));
        return services.BuildServiceProvider();
    }

    private static async Task<FilterRun> ExecuteAsync(
        RedisCacheAttribute attribute,
        HttpContext httpContext,
        IServiceProvider services,
        Func<IActionResult?> action)
    {
        httpContext.RequestServices = services;

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary());

        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object());

        var invoked = false;
        IActionResult? executedResult = null;

        ActionExecutionDelegate next = () =>
        {
            invoked = true;
            executedResult = action();

            var executed = new ActionExecutedContext(executingContext, new List<IFilterMetadata>(), new object())
            {
                Result = executedResult
            };

            return Task.FromResult(executed);
        };

        await attribute.OnActionExecutionAsync(executingContext, next);

        // A cache hit short-circuits by setting Result on the executing context;
        // otherwise the outcome comes from the action that actually ran.
        return new FilterRun(executingContext.Result ?? executedResult, invoked);
    }

    [Fact]
    public async Task DifferentUsers_AreStoredSeparately_WhenVaryByUserIsOn()
    {
        var cache = new FakeCacheServices();
        var services = CreateServices(cache);
        var attribute = new RedisCacheAttribute();

        await ExecuteAsync(attribute, CreateHttpContext("GET", "/api/Products", userId: "user-a"), services,
            () => new OkObjectResult(new { pageIndex = 1 }));
        await ExecuteAsync(attribute, CreateHttpContext("GET", "/api/Products", userId: "user-b"), services,
            () => new OkObjectResult(new { pageIndex = 1 }));

        Assert.Equal(2, cache.Store.Count);
        Assert.All(cache.Store.Keys, key => Assert.Contains("u:user-", key, StringComparison.Ordinal));
    }

    [Fact]
    public async Task EveryoneSharesOneEntry_WhenVaryByUserIsOff()
    {
        var cache = new FakeCacheServices();
        var services = CreateServices(cache);
        var attribute = new RedisCacheAttribute { VaryByUser = false };

        await ExecuteAsync(attribute, CreateHttpContext("GET", "/api/Products", userId: "user-a"), services,
            () => new OkObjectResult(new { pageIndex = 1 }));
        await ExecuteAsync(attribute, CreateHttpContext("GET", "/api/Products", userId: "user-b"), services,
            () => new OkObjectResult(new { pageIndex = 1 }));

        Assert.Single(cache.Store);
        Assert.DoesNotContain(cache.Store.Keys.Single(), "u:", StringComparison.Ordinal);
    }

    [Fact]
    public async Task AnonymousAndSignedInUsers_StillGetSeparateEntries_WhenVaryByUserIsOn()
    {
        var cache = new FakeCacheServices();
        var services = CreateServices(cache);
        var attribute = new RedisCacheAttribute();

        await ExecuteAsync(attribute, CreateHttpContext("GET", "/api/Products"), services,
            () => new OkObjectResult(new { pageIndex = 1 }));
        await ExecuteAsync(attribute, CreateHttpContext("GET", "/api/Products", userId: "user-a"), services,
            () => new OkObjectResult(new { pageIndex = 1 }));

        Assert.Equal(2, cache.Store.Count);
    }

    [Fact]
    public async Task DifferentQueryStrings_ProduceDifferentEntries()
    {
        var cache = new FakeCacheServices();
        var services = CreateServices(cache);
        var attribute = new RedisCacheAttribute { VaryByUser = false };

        await ExecuteAsync(attribute, CreateHttpContext("GET", "/api/Products", query: "?page=1"), services,
            () => new OkObjectResult(new { pageIndex = 1 }));
        await ExecuteAsync(attribute, CreateHttpContext("GET", "/api/Products", query: "?page=2"), services,
            () => new OkObjectResult(new { pageIndex = 2 }));

        Assert.Equal(2, cache.Store.Count);
    }

    [Fact]
    public async Task CachedPayload_UsesMvcCamelCasing_NotPascalCase()
    {
        var cache = new FakeCacheServices();
        var services = CreateServices(cache);

        await ExecuteAsync(new RedisCacheAttribute { VaryByUser = false },
            CreateHttpContext("GET", "/api/Products"), services,
            () => new OkObjectResult(new { PageIndex = 1, TotalCount = 10 }));

        var payload = Assert.Single(cache.Store.Values);

        using var document = JsonDocument.Parse(payload);
        Assert.True(document.RootElement.TryGetProperty("pageIndex", out _));
        Assert.True(document.RootElement.TryGetProperty("totalCount", out _));
        Assert.False(document.RootElement.TryGetProperty("PageIndex", out _));
    }

    [Fact]
    public async Task Errors_AreNeverCached()
    {
        var cache = new FakeCacheServices();
        var services = CreateServices(cache);

        var run = await ExecuteAsync(new RedisCacheAttribute { VaryByUser = false },
            CreateHttpContext("GET", "/api/Products/missing"), services,
            () => new NotFoundResult());

        Assert.True(run.NextInvoked);
        Assert.IsType<NotFoundResult>(run.Result);
        Assert.Empty(cache.Store);
    }

    [Fact]
    public async Task CacheHit_SkipsTheAction_AndReturnsTheCachedJson()
    {
        var cache = new FakeCacheServices();
        var services = CreateServices(cache);
        var attribute = new RedisCacheAttribute { VaryByUser = false };

        var first = await ExecuteAsync(attribute, CreateHttpContext("GET", "/api/Products"), services,
            () => new OkObjectResult(new { pageIndex = 1 }));
        Assert.True(first.NextInvoked);

        var second = await ExecuteAsync(attribute, CreateHttpContext("GET", "/api/Products"), services,
            () => new OkObjectResult(new { pageIndex = 999 }));

        Assert.False(second.NextInvoked);

        var content = Assert.IsType<ContentResult>(second.Result);
        Assert.Equal(StatusCodes.Status200OK, content.StatusCode);
        Assert.Equal("application/json", content.ContentType);
        Assert.Contains("pageIndex", content.Content, StringComparison.Ordinal);
        Assert.DoesNotContain("999", content.Content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task NonGetRequests_BypassTheCacheEntirely()
    {
        var cache = new FakeCacheServices();
        var services = CreateServices(cache);

        var run = await ExecuteAsync(new RedisCacheAttribute(), CreateHttpContext("POST", "/api/Orders"), services,
            () => new OkObjectResult(new { id = 1 }));

        Assert.True(run.NextInvoked);
        Assert.Empty(cache.Store);
    }

    [Fact]
    public async Task CacheServices_CanInvalidateByExactKeyAndByPrefix()
    {
        var repository = new InMemoryCacheRepository();
        var services = new ECommerce.Application.Services.Classes.Cache.CacheServices(repository);

        await services.SetAsync("cache:http:GET:/api/Products", "a", TimeSpan.FromMinutes(5));
        await services.SetAsync("cache:http:GET:/api/Products?page=2", "b", TimeSpan.FromMinutes(5));
        await services.SetAsync("other:key", "c", TimeSpan.FromMinutes(5));

        await services.RemoveAsync("cache:http:GET:/api/Products");
        Assert.Null(await services.GetAsync("cache:http:GET:/api/Products"));
        Assert.NotNull(await services.GetAsync("cache:http:GET:/api/Products?page=2"));

        await services.RemoveByPrefixAsync("cache:http:");
        Assert.Null(await services.GetAsync("cache:http:GET:/api/Products?page=2"));
        Assert.NotNull(await services.GetAsync("other:key"));
    }

    private sealed class InMemoryCacheRepository : ECommerce.Domain.Contracts.Repositories.ICacheRepository
    {
        private readonly Dictionary<string, string> _store = new(StringComparer.Ordinal);

        public Task<string?> GetAsync(string key, CancellationToken ct = default)
            => Task.FromResult(_store.TryGetValue(key, out var value) ? value : null);

        public Task SetAsync(string key, string value, TimeSpan? duration = null, CancellationToken ct = default)
        {
            _store[key] = value;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken ct = default)
        {
            _store.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
        {
            foreach (var key in _store.Keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList())
                _store.Remove(key);

            return Task.CompletedTask;
        }
    }
}
