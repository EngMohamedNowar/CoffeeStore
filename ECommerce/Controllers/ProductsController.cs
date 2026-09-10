using CoffeeStore.Application.DTOs.Products;
using ECommerce.Application.Services.Classes.Products;
using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    public class ProductsController(IProductService productService) : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAllProducts(CancellationToken ct = default)
        {
            var result = await productService.GetAllActiveAsync(ct);
            return ToActionResult(result);
        }
        [HttpGet("{slug}")]
        public async Task<ActionResult<ProductDto>> GetProductBySlug(string slug,CancellationToken ct = default)
        {
            var result = await productService.GetBySlugAsync(slug,ct);
            return ToActionResult(result);
        }
    }
}
