using CoffeeStore.Domain.Entities.Categories;
using CoffeeStore.Domain.Entities.Customers;
using CoffeeStore.Domain.Entities.Products;
using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts;
using ECommerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.Infrastructure.Persistence.DataSeeding
{
    public class CatalogDataSeeder(StoreDbContext context,ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedDataAsync(CancellationToken ct)
        {
            try
            {
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync(ct);
                if (pendingMigrations.Any())
                    await context.Database.MigrateAsync(ct);

                var rootPath = Path.Combine(AppContext.BaseDirectory, "DataSeeding");

                await SeedIfEmpty<Category>(rootPath, "categories.json", ct);
                await context.SaveChangesAsync(ct); // احفظ الـ Categories الأول عشان تاخد Ids حقيقية

                await SeedIfEmpty<Product>(rootPath, "products.json", ct);
                await context.SaveChangesAsync(ct); // احفظ الـ Products قبل ما تضيف Variants اللي بتعتمد عليها

                logger.LogInformation("Seeding completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }
        
        private async Task SeedIfEmpty<TEntity>(string rootPath, string fileName, CancellationToken ct = default) where TEntity : BaseEntity
        {
            if (await context.Set<TEntity>().AnyAsync())
            {
                logger.LogInformation("Table Has Data");
                return;
            }
            var filePath = Path.Combine(rootPath, fileName);
            if (!File.Exists(filePath))
            {
                logger.LogWarning($"File {fileName} Not Exists");
                return;
            }
            // seeding
            var fileStream = File.OpenRead(filePath);

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var data = await JsonSerializer.DeserializeAsync<List<TEntity>>(fileStream,options,ct); // convert json data into objects

            if (data is not null && data.Any())
            {
                await context.Set<TEntity>().AddRangeAsync(data);
            }
        }


    }
}
