using ECommerce.Api.Extensions;
using ECommerce.Application;
using ECommerce.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["TraceId"] = context.HttpContext.TraceIdentifier;

        if (context.Exception is not null)
        {
            context.ProblemDetails.Extensions["Exception"] = context.Exception.GetType().Name;
        }
    };
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ECommerce.Api.Filters.ValidationFilter>();
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ECommerce API",
        Version = "v1",
        Description = "ECommerce API Documentation"
    });
});

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<ECommerce.Api.Middleware.RequestLoggingMiddleware>();

await app.SeedAndMigrationAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API V1");
        options.RoutePrefix = "swagger";
    });
}

var staticFilesRoot = Path.Combine(builder.Environment.ContentRootPath, "Files");
Directory.CreateDirectory(staticFilesRoot);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(staticFilesRoot),
    RequestPath = "/Files"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
