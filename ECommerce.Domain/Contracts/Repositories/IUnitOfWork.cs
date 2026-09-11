using CoffeeStore.Domain.Entities.Products;
using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct);
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
        IProductRepository ProductRepository();
    }
}
