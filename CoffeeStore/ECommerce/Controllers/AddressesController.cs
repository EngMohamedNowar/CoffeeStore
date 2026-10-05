using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Api.Controllers;

[Authorize]
public class AddressesController(IIdentityService identityService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AddressDto>>> GetAll(CancellationToken ct = default)
    {
        var result = await identityService.GetAddressesAsync(GetEmail(), ct);
        return ToActionResult(result);
    }

    [HttpGet("default")]
    public async Task<ActionResult<AddressDto>> GetDefault(CancellationToken ct = default)
    {
        var result = await identityService.GetCurrentUserAddressAsync(GetEmail(), ct);
        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<AddressDto>> Create([FromBody] AddressDto address, CancellationToken ct = default)
    {
        var result = await identityService.AddAddressAsync(GetEmail(), address, ct);
        return ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AddressDto>> Update(Guid id, [FromBody] AddressDto address, CancellationToken ct = default)
    {
        var result = await identityService.UpdateAddressAsync(GetEmail(), id, address, ct);
        return ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var result = await identityService.DeleteAddressAsync(GetEmail(), id, ct);
        return ToActionResult(result);
    }

    private string GetEmail()
        => User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException();
}
