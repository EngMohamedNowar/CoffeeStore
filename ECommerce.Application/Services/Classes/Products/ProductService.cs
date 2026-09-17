using AutoMapper;
using CoffeeStore.Application.DTOs.Products;
using CoffeeStore.Domain.Entities.Products;
using ECommerce.Application.Common;
using ECommerce.Application.Services.Contracts;
using ECommerce.Application.Specifications;
using ECommerce.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services.Classes.Products
{
    public class ProductService(IUnitOfWork unitOfWork,IMapper mapper) : IProductService
    {
        public async Task<Result<PaginationResult<ProductDto>>> GetAllActiveAsync(ProductQueryParams queryParams, CancellationToken ct = default)
        {
            var specs = new ProductsWithCategory(queryParams);
            var products = await unitOfWork.GetRepository<Product>().GetAllAsync(specs,ct);
            var countSpecs = new ProductCountSpecifications(queryParams);
            var productsDtos = mapper.Map<IReadOnlyList<ProductDto>>(products);
            var count = await unitOfWork.GetRepository<Product>().CountAsync(countSpecs, ct);
            var result = new PaginationResult<ProductDto>(queryParams.PageIndex, queryParams.PageSize, count, productsDtos);
            return Result<PaginationResult<ProductDto>>.Ok(result);
        }

        public async Task<Result<ProductDto?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            {
                var specs = new ProductBySlugWithCategorySpecification(slug);
                var product = await unitOfWork.ProductRepository().GetBySlugAsync(slug,specs,cancellationToken);
                var productDtos = mapper.Map<ProductDto>(product);
                return Result<ProductDto>.Ok(productDtos);
            }
        }

        public Task<Result<Guid>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public Task<Result<bool>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


    }
}
