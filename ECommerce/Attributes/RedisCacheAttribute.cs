using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace ECommerce.Api.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheServices = context.HttpContext.RequestServices.GetRequiredService<ICacheServices>();
            var cacheKey = CreateCacheKey(context.HttpContext.Request);
            var value = await cacheServices.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(value))
            {
                context.Result = new ContentResult()
                {
                    Content = value,
                    StatusCode = 200,
                    ContentType = "application/json"
                };
                return;
            }
            else
            {
                var excuted = await next.Invoke();
                if (excuted.Result is OkObjectResult okObjectResult)
                {
                    await cacheServices.SetAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(1000));
                }
            }

        }

        private static string CreateCacheKey(HttpRequest request)
        {
            var key = new StringBuilder();
            key.Append(request.Path);

            foreach (var item in request.Query)
            {
                key.Append($"{item.Key} | {item.Value}");

            }
            return key.ToString();
        }
    }

    
}
