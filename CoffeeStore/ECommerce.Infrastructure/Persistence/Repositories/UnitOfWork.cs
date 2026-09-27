using ECommerce.Domain.Common;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    internal class UnitOfWork(StoreDbContext context) : IUnitOfWork
    {
        private readonly Dictionary<string, object> _repositories = [];

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            var typeOfRepo = typeof(TEntity).Name;
            if (_repositories.TryGetValue(typeOfRepo, out object? value))
            {
                return (IGenericRepository<TEntity>)value;
            }
            var repo = new GenericRepository<TEntity>(context);
            _repositories.Add(typeOfRepo, repo);
            return repo;
        }

        public IProductRepository ProductRepository()
        {
            var typeOfRepo = nameof(IProductRepository);
            if (_repositories.TryGetValue(typeOfRepo, out object? value))
            {
                return (IProductRepository)value;
            }
            var repo = new ProductRepository(context);
            _repositories.Add(typeOfRepo, repo);
            return repo;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
         => await context.SaveChangesAsync(ct);
    }
}