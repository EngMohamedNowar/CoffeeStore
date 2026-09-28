using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RedisCacheAttribute : ActionFilterAttribute
{
    private const string KeyPrefix = "cache:http:";

    public int DurationInSeconds { get; set; } = 60;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheServices = context.HttpContext.RequestServices.GetRequiredService<ICacheServices>();
        var cacheKey = CreateCacheKey(context.HttpContext.Request);

        var cached = await cacheServices.GetAsync(cacheKey);
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

        var executed = await next.Invoke();

        if (executed.Result is OkObjectResult { Value: not null } okObjectResult)
        {
            await cacheServices.SetAsync(
                cacheKey,
                okObjectResult.Value,
                TimeSpan.FromSeconds(DurationInSeconds));
        }
    }

    private static string CreateCacheKey(HttpRequest request)
    {
        var query = request.Query
            .OrderBy(q => q.Key, StringComparer.Ordinal)
            .Select(q => $"{q.Key}={q.Value.ToString()}");

        return KeyPrefix + request.Path + (query.Any() ? "?" + string.Join("&", query) : string.Empty);
    }
}
