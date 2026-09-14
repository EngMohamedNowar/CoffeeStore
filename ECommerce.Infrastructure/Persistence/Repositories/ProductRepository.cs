using CoffeeStore.Domain.Entities.Products;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Specification;
using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Specifications;
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

        public async Task<Product?> GetBySlugAsync(string slug, ISpecification<Product> specs, CancellationToken ct = default)
        {
            var query = await SpecificationEvaluator.CreateQuery(context.Set<Product>(), specs).FirstOrDefaultAsync(f=>f.Slug==slug);
            return query;
        }
    }
}

