using CoffeeStore.Domain.Entities.Products;
using ECommerce.Domain.Contracts.Repositories;

namespace ECommerce.Domain.Contracts.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetBySlugAsync(
        string slug,
        CancellationToken ct = default);
}