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
        [FromBody] CreateOrderRequest request,
        CancellationToken ct = default)
    {
        var command = new CreateOrderCommand(
            request.BasketId,
            request.DeliveryMethodId,
            GetEmail(),
            request.FirstName,
            request.LastName,
            request.Street,
            request.City,
            request.Country);

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

public class CreateOrderRequest
{
    public Guid BasketId { get; set; }
    public Guid DeliveryMethodId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
