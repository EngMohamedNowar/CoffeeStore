using ECommerce.Api.Attributes;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

public class ProductsController(IProductService productService) : ApiControllerBase
{
    [HttpGet]
    [RedisCache]
    public async Task<ActionResult<PaginationResult<ProductDto>>> GetAllProducts(
        [FromQuery] ProductQueryParams queryParams,
        CancellationToken ct = default)
    {
        var result = await productService.GetAllActiveAsync(queryParams, ct);
        return ToActionResult(result);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ProductDto>> GetProductBySlug(string slug, CancellationToken ct = default)
    {
        var result = await productService.GetBySlugAsync(slug, ct);
        return ToActionResult(result);
    }
}
