using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Specification;
using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity>(StoreDbContext context) : IGenericRepository<TEntity>
    where TEntity : BaseEntity
{
    private readonly StoreDbContext _context = context;

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
        => await _context.Set<TEntity>().ToListAsync(ct);

    public void Add(TEntity entity)
        => _context.Set<TEntity>().Add(entity);

    public void Update(TEntity entity)
        => _context.Set<TEntity>().Update(entity);

    public void Delete(TEntity entity)
        => _context.Set<TEntity>().Remove(entity);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity> specs, CancellationToken ct = default)
        => await SpecificationEvaluator.CreateQuery(_context.Set<TEntity>(), specs).ToListAsync(ct);

    public async Task<int> CountAsync(ISpecification<TEntity> specs, CancellationToken ct = default)
        => await SpecificationEvaluator.CreateCountQuery(_context.Set<TEntity>(), specs).CountAsync(ct);
}
