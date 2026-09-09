using CoffeeStore.Application.DTOs.Products;
using ECommerce.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services.Contracts;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}


