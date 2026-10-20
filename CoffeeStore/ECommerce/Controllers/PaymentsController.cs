using System.Security.Claims;
using ECommerce.Application.Common.Messaging;
using ECommerce.Application.Payments.Commands.CreatePaymentIntent;
using ECommerce.Application.Payments.Commands.HandlePaymentWebhook;
using ECommerce.Application.Payments.Dtos;
using ECommerce.Application.Payments.Queries.GetPaymentByOrderId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

public class PaymentsController(
    IRequestDispatcher requestDispatcher,
    IQueryDispatcher queryDispatcher) : ApiControllerBase
{
    [Authorize]
    [HttpPost("intent")]
    public async Task<ActionResult<PaymentResponse>> CreatePaymentIntent(
        [FromBody] CreatePaymentIntentDto dto,
        CancellationToken ct = default)
    {
        var command = new CreatePaymentIntentCommand(dto.OrderId, GetEmail());
        var result = await requestDispatcher.SendAsync(command, ct);
        return ToActionResult(result);
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(CancellationToken ct = default)
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync(ct);
        var signature = Request.Headers["Stripe-Signature"].ToString();

        var command = new HandlePaymentWebhookCommand(json, signature);
        var result = await requestDispatcher.SendAsync(command, ct);

        return result.IsSuccess ? Ok() : ToProblem(result.Errors);
    }

    [Authorize]
    [HttpGet("order/{orderId:guid}")]
    public async Task<ActionResult<PaymentResponse>> GetPaymentByOrderId(
        Guid orderId,
        CancellationToken ct = default)
    {
        var result = await queryDispatcher.SendAsync(
            new GetPaymentByOrderIdQuery(orderId, GetEmail()), ct);
        return ToActionResult(result);
    }

    private string GetEmail()
        => User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException();
}
