using ECommerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerce.Infrastructure.Persistence.DesignTime;

public sealed class StoreDbContextDesignTimeFactory : IDesignTimeDbContextFactory<StoreDbContext>
{
    public StoreDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseSqlServer("Server=localhost;Database=CoffeeStoreDb;TrustServerCertificate=True;")
            .Options;

        return new StoreDbContext(options);
    }
}
