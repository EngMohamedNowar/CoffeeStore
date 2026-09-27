using ECommerce.Domain.Common;
using ECommerce.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Contracts.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity 
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity> specs ,CancellationToken ct = default);
        Task<int> CountAsync(ISpecification<TEntity> specs, CancellationToken ct = default);




    }
}
