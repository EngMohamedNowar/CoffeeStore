using ECommerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Persistence.DesignTime;

public sealed class StoreDbContextDesignTimeFactory : IDesignTimeDbContextFactory<StoreDbContext>
{
    public StoreDbContext CreateDbContext(string[] args)
    {
        var apiSecretsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft", "UserSecrets", "5b0d9786-f1fe-4331-a567-b865b4836147", "secrets.json");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "ECommerce"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile(apiSecretsPath, optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new StoreDbContext(options);
    }
}
