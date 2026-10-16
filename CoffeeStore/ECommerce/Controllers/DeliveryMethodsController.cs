using ECommerce.Application.Common.Messaging;
using ECommerce.Application.DeliveryMethods.Commands.CreateDeliveryMethod;
using ECommerce.Application.DeliveryMethods.Commands.DeleteDeliveryMethod;
using ECommerce.Application.DeliveryMethods.Commands.UpdateDeliveryMethod;
using ECommerce.Application.DeliveryMethods.Dtos;
using ECommerce.Application.DeliveryMethods.Queries.GetAllDeliveryMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

public class DeliveryMethodsController(
    IQueryDispatcher queryDispatcher,
    IRequestDispatcher requestDispatcher) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethodResponse>>> GetAllDeliveryMethods(
        CancellationToken ct = default)
    {
        var result = await queryDispatcher.SendAsync(new GetAllDeliveryMethodsQuery(), ct);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<DeliveryMethodResponse>> CreateDeliveryMethod(
        [FromBody] CreateDeliveryMethodDto dto,
        CancellationToken ct = default)
    {
        var command = new CreateDeliveryMethodCommand(
            dto.Name, dto.Price, dto.EstimatedDeliveryTime,
            dto.Description, dto.IsAvailable, dto.DisplayOrder);

        var result = await requestDispatcher.SendAsync(command, ct);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DeliveryMethodResponse>> UpdateDeliveryMethod(
        Guid id,
        [FromBody] UpdateDeliveryMethodDto dto,
        CancellationToken ct = default)
    {
        var command = new UpdateDeliveryMethodCommand(
            id, dto.Name, dto.Price, dto.EstimatedDeliveryTime,
            dto.Description, dto.IsAvailable, dto.DisplayOrder);

        var result = await requestDispatcher.SendAsync(command, ct);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeleteDeliveryMethod(
        Guid id,
        CancellationToken ct = default)
    {
        var result = await requestDispatcher.SendAsync(new DeleteDeliveryMethodCommand(id), ct);
        return ToActionResult(result);
    }
}
