using CoffeeStore.Domain.Entities.Products;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class ProductRepository(StoreDbContext context)
        : GenericRepository<Product>(context), IProductRepository
    {
        public async Task<Product?> GetBySlugAsync(
            string slug,
            CancellationToken ct = default)
            => await context.Set<Product>()
                .FirstOrDefaultAsync(p => p.Slug == slug, ct);
    }
}