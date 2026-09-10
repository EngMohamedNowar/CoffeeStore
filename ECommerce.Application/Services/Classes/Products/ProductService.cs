using AutoMapper;
using CoffeeStore.Application.DTOs.Products;
using CoffeeStore.Domain.Entities.Products;
using ECommerce.Application.Common;
using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services.Classes.Products
{
    public class ProductService(IUnitOfWork unitOfWork,IMapper mapper) : IProductService
    {
        public async Task<Result<IReadOnlyList<ProductDto>>> GetAllActiveAsync(CancellationToken ct = default)
        {
            var products = await unitOfWork.GetRepository<Product>().GetAllAsync(ct);
            var productsDtos = mapper.Map<IReadOnlyList<ProductDto>>(products);
            return Result<IReadOnlyList<ProductDto>>.Ok(productsDtos);
        }

        public async Task<Result<ProductDto?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var product = await unitOfWork.GetRepository<Product>().GetBySlugAsync(slug,cancellationToken); 
            var productDtos = mapper.Map<ProductDto>(product);
            if (productDtos is null)
            {
                return Result<ProductDto>.Fail(Error.NotFound($"{productDtos.Slug} NOT FOUND"));
            }
            return Result<ProductDto>.Ok(productDtos);
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
