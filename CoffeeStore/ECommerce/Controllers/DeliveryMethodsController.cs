using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Dtos;
using ECommerce.Application.DeliveryMethods.Queries.GetAllDeliveryMethods;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

public class DeliveryMethodsController(IQueryDispatcher queryDispatcher) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethodResponse>>> GetAllDeliveryMethods(
        CancellationToken ct = default)
    {
        var result = await queryDispatcher.SendAsync(new GetAllDeliveryMethodsQuery(), ct);
        return ToActionResult(result);
    }
}
