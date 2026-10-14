using ECommerce.Domain.Common;
using ECommerce.Domain.Specification;

namespace ECommerce.Domain.Contracts.Repositories;

public interface IReadRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
}
