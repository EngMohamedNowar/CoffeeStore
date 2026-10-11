using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ECommerce.Infrastructure.Persistence.DataSeeding;

public class DeliveryMethodDataSeeder(
    StoreDbContext context,
    ILogger<DeliveryMethodDataSeeder> logger) : IDataSeeder
{
    private const string SeedFileName = "delivery-methods.json";

    public async Task SeedDataAsync(CancellationToken ct = default)
    {
        if (await context.DeliveryMethods.AnyAsync(ct))
        {
            logger.LogInformation(
                "DeliveryMethods table already contains data. Skipping seeding.");

            return;
        }

        var filePath = Path.Combine(AppContext.BaseDirectory, "DataSeeding", SeedFileName);

        if (!File.Exists(filePath))
        {
            logger.LogWarning(
                "Seeding file {FileName} was not found at {FilePath}.",
                SeedFileName,
                filePath);

            return;
        }

        await using var fileStream = File.OpenRead(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var data = await JsonSerializer.DeserializeAsync<List<DeliveryMethodSeedData>>(
            fileStream,
            options,
            ct);

        if (data is null || data.Count == 0)
        {
            logger.LogWarning("No data found in {FileName}.", SeedFileName);

            return;
        }

        var methods = new List<DeliveryMethod>();

        foreach (var entry in data)
        {
            var result = entry.ToDomain();

            if (result.IsFailure)
            {
                logger.LogWarning(
                    "Skipping delivery method {Name} in {FileName}: {Errors}",
                    entry.Name,
                    SeedFileName,
                    string.Join(", ", result.Errors.Select(error => error.code)));

                continue;
            }

            methods.Add(result.Value!);
        }

        if (methods.Count == 0)
            return;

        await context.DeliveryMethods.AddRangeAsync(methods, ct);
        await context.SaveChangesAsync(ct);

        logger.LogInformation(
            "Added {Count} DeliveryMethod records from {FileName}.",
            methods.Count,
            SeedFileName);
    }
}
