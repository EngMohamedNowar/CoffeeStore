using ECommerce.Application.DTOs.Baskets;
using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{

    public class BasketsController : ApiControllerBase
    {
        private readonly IBasketService _service;

        public BasketsController(IBasketService service)
        {
            _service = service;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BasketDto>> GetBasket(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetBasketAsync(
                id,
                cancellationToken);

            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateOrUpdateBasket(
            [FromBody] BasketDto basket,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.CreateOrUpdateBasketAsync(
                basket,
                null,
                cancellationToken);

            return ToActionResult(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> DeleteBasket(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.DeleteBasketAsync(
                id,
                cancellationToken);

            return ToActionResult(result);
        }
    }
}
