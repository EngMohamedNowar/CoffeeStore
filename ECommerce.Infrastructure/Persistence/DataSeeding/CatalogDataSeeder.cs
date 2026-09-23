using CoffeeStore.Domain.Entities.Categories;
using CoffeeStore.Domain.Entities.Products;
using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts;
using ECommerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.Infrastructure.Persistence.DataSeeding
{
    public class CatalogDataSeeder(
        StoreDbContext context,
        ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedDataAsync(CancellationToken ct)
        {
            try
            {
                // Apply pending migrations
                await context.Database.MigrateAsync(ct);

                var rootPath = Path.Combine(
                    AppContext.BaseDirectory,
                    "DataSeeding");

                // Seed Categories first
                await SeedIfEmpty<Category>(
                    rootPath,
                    "categories.json",
                    ct);

                await context.SaveChangesAsync(ct);

                // Seed Products after Categories
                await SeedIfEmpty<Product>(
                    rootPath,
                    "products.json",
                    ct);

                await context.SaveChangesAsync(ct);

                logger.LogInformation(
                    "Database migration and seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred while migrating or seeding the database.");

                throw;
            }
        }

        private async Task SeedIfEmpty<TEntity>(
            string rootPath,
            string fileName,
            CancellationToken ct = default)
            where TEntity : BaseEntity
        {
            // Check if the table already contains data
            if (await context.Set<TEntity>().AnyAsync(ct))
            {
                logger.LogInformation(
                    "{EntityName} table already contains data. Skipping seeding.",
                    typeof(TEntity).Name);

                return;
            }

            var filePath = Path.Combine(rootPath, fileName);

            // Check if JSON file exists
            if (!File.Exists(filePath))
            {
                logger.LogWarning(
                    "Seeding file {FileName} was not found at {FilePath}.",
                    fileName,
                    filePath);

                return;
            }

            // Read and deserialize JSON
            await using var fileStream = File.OpenRead(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(
                new JsonStringEnumConverter());

            var data = await JsonSerializer.DeserializeAsync<List<TEntity>>(
                fileStream,
                options,
                ct);

            if (data is null || data.Count == 0)
            {
                logger.LogWarning(
                    "No data found in {FileName}.",
                    fileName);

                return;
            }

            // Add data to DbContext
            await context.Set<TEntity>().AddRangeAsync(data, ct);

            logger.LogInformation(
                "Added {Count} {EntityName} records from {FileName}.",
                data.Count,
                typeof(TEntity).Name,
                fileName);
        }
    }
}
