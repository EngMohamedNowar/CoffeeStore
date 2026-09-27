using AutoMapper;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Services.Contracts;
using ECommerce.Application.Specifications;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.Products;

namespace ECommerce.Application.Services.Classes.Products;

public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
{
    public async Task<Result<PaginationResult<ProductDto>>> GetAllActiveAsync(ProductQueryParams queryParams, CancellationToken ct = default)
    {
        var products = await unitOfWork.GetRepository<Product>()
            .GetAllAsync(new ProductsWithCategory(queryParams), ct);

        var count = await unitOfWork.GetRepository<Product>()
            .CountAsync(new ProductCountSpecifications(queryParams), ct);

        var productsDtos = mapper.Map<IReadOnlyList<ProductDto>>(products);
        var result = new PaginationResult<ProductDto>(queryParams.PageIndex, queryParams.PageSize, count, productsDtos);

        return Result<PaginationResult<ProductDto>>.Ok(result);
    }

    public async Task<Result<ProductDto>> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var specs = new ProductBySlugWithCategorySpecification(slug);
        var product = await unitOfWork.ProductRepository().GetBySlugAsync(specs, ct);

        if (product is null)
        {
            return Result<ProductDto>.Fail(Error.NotFound(
                "Product.NotFound",
                $"No product was found for slug '{slug}'."));
        }

        return Result<ProductDto>.Ok(mapper.Map<ProductDto>(product));
    }

    public Task<Result<Guid>> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Result<bool>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
        => throw new NotImplementedException();
}
