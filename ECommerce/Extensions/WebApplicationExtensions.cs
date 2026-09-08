using ECommerce.Domain.Contracts;

namespace ECommerce.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> SeedAndMigrationAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dataSeeder = scope.ServiceProvider.GetRequiredKeyedService<IDataSeeder>("Catalog");
            await dataSeeder.SeedDataAsync();
            return app;
        }
    }
}
