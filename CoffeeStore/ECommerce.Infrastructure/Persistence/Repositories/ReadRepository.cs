using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Specification;
using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class ReadRepository<TEntity>(StoreDbContext context) : IReadRepository<TEntity>
    where TEntity : BaseEntity
{
    public async Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        => await SpecificationEvaluator
            .CreateQuery(context.Set<TEntity>(), specification, applyPaging: false)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        => await SpecificationEvaluator.CreateQuery(context.Set<TEntity>(), specification).ToListAsync(ct);

    public async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        => await SpecificationEvaluator.CreateCountQuery(context.Set<TEntity>(), specification).CountAsync(ct);
}
