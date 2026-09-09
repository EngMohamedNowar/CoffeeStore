using AutoMapper;
using CoffeeStore.Application.DTOs.Products;
using ECommerce.Application.Common;
using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services.Classes
{
    public class ProductService(IUnitOfWork unitOfWork,IMapper mapper) : IProductService
    {
        public Task<IReadOnlyList<ProductDto>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
