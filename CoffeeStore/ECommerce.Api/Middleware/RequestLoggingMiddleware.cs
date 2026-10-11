using System.Diagnostics;

namespace ECommerce.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        _logger.LogInformation("Request {Method} {Path} started", requestMethod, requestPath);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;

            if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "Request {Method} {Path} finished with status {StatusCode} in {ElapsedMs}ms",
                    requestMethod, requestPath, statusCode, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation(
                    "Request {Method} {Path} finished with status {StatusCode} in {ElapsedMs}ms",
                    requestMethod, requestPath, statusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
