using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Specification;
using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(StoreDbContext context) : base(context)
    {
        Context = context;
    }

    private StoreDbContext Context { get; }

    public Task<Product?> GetBySlugAsync(ISpecification<Product> specs, CancellationToken ct = default)
        => SpecificationEvaluator.CreateQuery(Context.Set<Product>(), specs).FirstOrDefaultAsync(ct);

    public Task<Product?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => Context.Set<Product>().FirstOrDefaultAsync(p => p.Slug == slug, ct);
}
