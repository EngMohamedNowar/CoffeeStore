using CoffeeStore.Domain.Entities.Products;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Specification;

namespace ECommerce.Domain.Contracts.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetBySlugAsync(
        string slug,
        ISpecification<Product> specs,
        CancellationToken ct = default);

    Task<Product?> GetBySlugAsync(
    string slug,
    CancellationToken ct = default);
}