using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity>(StoreDbContext context) : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
             => await context.Set<TEntity>().ToListAsync(ct);

        public async Task<TEntity?> GetBySlugAsync(
            string slug,
            CancellationToken ct = default)
        
            => await context.Set<TEntity>()
                .FirstOrDefaultAsync(
                    x => EF.Property<string>(x, "Slug") == slug,
                    ct);
        

        public void Add(TEntity entity) => context.Set<TEntity>().Add(entity);

        public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);

        public void Delete(TEntity entity) => context.Set<TEntity>().Remove(entity);
    }
}
