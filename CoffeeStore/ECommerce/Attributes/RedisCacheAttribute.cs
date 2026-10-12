using System.Security.Claims;
using System.Text.Json;
using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace ECommerce.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RedisCacheAttribute : ActionFilterAttribute
{
    private const string KeyPrefix = "cache:http:";

    public int DurationInSeconds { get; set; } = 60;

    /// <summary>
    /// When <c>true</c> (the safe default) the cache key is scoped to the signed-in
    /// user, so one user's response is never served to another. Set it to
    /// <c>false</c> only for endpoints whose response is identical for everyone
    /// (public/shared reads) so all callers share a single cache entry.
    /// </summary>
    public bool VaryByUser { get; set; } = true;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        // Only GET is idempotent and safe to replay from the cache.
        if (!HttpMethods.IsGet(httpContext.Request.Method))
        {
            await next();
            return;
        }

        var cacheServices = httpContext.RequestServices.GetRequiredService<ICacheServices>();
        var cacheKey = CreateCacheKey(httpContext, VaryByUser);

        var cached = await cacheServices.GetAsync(cacheKey, httpContext.RequestAborted);
        if (!string.IsNullOrEmpty(cached))
        {
            context.Result = new ContentResult
            {
                Content = cached,
                StatusCode = StatusCodes.Status200OK,
                ContentType = "application/json"
            };
            return;
        }

        var executed = await next();

        // Errors are never cached; only a 200 with a payload is.
        if (executed.Result is OkObjectResult { Value: not null } okObjectResult)
        {
            // Serialize with MVC's own JsonOptions so the cached bytes are
            // identical to what the live response produces (camelCase, same
            // converters), instead of the PascalCase default the repository used.
            var jsonOptions = httpContext.RequestServices
                .GetRequiredService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>()
                .Value.JsonSerializerOptions;

            var payload = JsonSerializer.Serialize(
                okObjectResult.Value,
                okObjectResult.Value.GetType(),
                jsonOptions);

            await cacheServices.SetAsync(
                cacheKey,
                payload,
                TimeSpan.FromSeconds(DurationInSeconds),
                httpContext.RequestAborted);
        }
    }

    internal static string CreateCacheKey(HttpContext httpContext, bool varyByUser)
    {
        var request = httpContext.Request;

        var query = request.Query
            .OrderBy(q => q.Key, StringComparer.Ordinal)
            .Select(q => $"{q.Key}={q.Value.ToString()}");

        var path = request.Path.Value ?? "/";

        var key = KeyPrefix
            + request.Method
            + ":"
            + path
            + (query.Any() ? "?" + string.Join("&", query) : string.Empty);

        if (varyByUser)
        {
            key += "|u:" + ResolveUserId(httpContext.User);
        }

        return key;
    }

    private static string ResolveUserId(ClaimsPrincipal? user)
    {
        if (user?.Identity?.IsAuthenticated != true)
            return "anonymous";

        return user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? user.FindFirstValue(ClaimTypes.Email)
            ?? "anonymous";
    }
}
