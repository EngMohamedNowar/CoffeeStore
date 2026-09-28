using ECommerce.Application.DTOs.Baskets;
using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

public class BasketsController(IBasketService service) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BasketDto>> GetBasket(Guid id, CancellationToken ct = default)
    {
        var result = await service.GetBasketAsync(id, ct);
        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<BasketDto>> CreateOrUpdateBasket([FromBody] BasketDto basket, CancellationToken ct = default)
    {
        var result = await service.CreateOrUpdateBasketAsync(basket, null, ct);
        return ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeleteBasket(Guid id, CancellationToken ct = default)
    {
        var result = await service.DeleteBasketAsync(id, ct);
        return ToActionResult(result);
    }
}
