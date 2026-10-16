using System.Security.Claims;
using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Orders.Commands.CreateOrder;
using ECommerce.Application.Orders.Dtos;
using ECommerce.Application.Orders.Queries.GetOrderById;
using ECommerce.Application.Orders.Queries.GetOrdersByEmail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[Authorize]
public class OrderController(
    IRequestDispatcher requestDispatcher,
    IQueryDispatcher queryDispatcher) : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        [FromBody] CreateOrderDto dto,
        CancellationToken ct = default)
    {
        var command = new CreateOrderCommand(
            dto.BasketId,
            dto.DeliveryMethodId,
            GetEmail(),
            dto.FirstName,
            dto.LastName,
            dto.Street,
            dto.City,
            dto.Country);

        var result = await requestDispatcher.SendAsync(command, ct);
        return ToActionResult(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetMyOrders(
        CancellationToken ct = default)
    {
        var result = await queryDispatcher.SendAsync(
            new GetOrdersByEmailQuery(GetEmail()), ct);
        return ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetOrderById(
        Guid id,
        CancellationToken ct = default)
    {
        var result = await queryDispatcher.SendAsync(new GetOrderByIdQuery(id), ct);
        return ToActionResult(result);
    }

    private string GetEmail()
        => User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException();
}
