using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Persistence.DataSeeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace ECommerce.Tests.Infrastructure;

public class DeliveryMethodDataSeederTests
{
    private static StoreDbContext BuildStoreContext()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new StoreDbContext(options);
    }

    private static string SeedFilePath
        => Path.Combine(AppContext.BaseDirectory, "DataSeeding", "delivery-methods.json");

    [Fact]
    public void ShippedSeedFile_MapsEveryEntryToAValidDeliveryMethod()
    {
        Assert.True(File.Exists(SeedFilePath), $"Seed file not found at {SeedFilePath}");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var data = JsonSerializer.Deserialize<List<DeliveryMethodSeedData>>(
            File.ReadAllText(SeedFilePath),
            options);

        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var methods = data
            .Select(entry => entry.ToDomain())
            .ToList();

        Assert.All(
            methods,
            result => Assert.True(
                result.IsSuccess,
                string.Join(", ", result.Errors.Select(error => error.code))));

        Assert.Equal(data.Count, methods.Select(result => result.Value!.Id).Distinct().Count());
        Assert.Equal(data.Count, methods.Select(result => result.Value!.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public async Task Seeder_PopulatesDeliveryMethodsOnlyOnce()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var expectedCount = JsonSerializer.Deserialize<List<DeliveryMethodSeedData>>(
            File.ReadAllText(SeedFilePath),
            options)!.Count;

        using var context = BuildStoreContext();
        var seeder = new DeliveryMethodDataSeeder(
            context,
            NullLogger<DeliveryMethodDataSeeder>.Instance);

        await seeder.SeedDataAsync();
        await seeder.SeedDataAsync();

        var seeded = await context.DeliveryMethods.OrderBy(method => method.DisplayOrder).ToListAsync();

        Assert.Equal(expectedCount, seeded.Count);
        Assert.Equal(seeded.Select(method => method.Id).Distinct().Count(), seeded.Count);
        Assert.All(seeded, method => Assert.True(method.IsAvailable));
        Assert.Contains(seeded, method => method.Name == "Store Pickup" && method.Price == 0m);
        Assert.Contains(seeded, method => method.Name == "Express Delivery" && method.Price == 60m);
    }
}
