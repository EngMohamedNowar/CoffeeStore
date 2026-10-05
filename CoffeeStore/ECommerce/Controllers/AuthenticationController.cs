using System.Security.Claims;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

public class AuthenticationController(IAuthenticationService authentication) : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto login, CancellationToken ct = default)
    {
        var result = await authentication.LoginAsync(login, ct);
        return ToActionResult(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegistrationDto registration, CancellationToken ct = default)
    {
        var result = await authentication.RegistrationAsync(registration, ct);
        return ToActionResult(result);
    }

    [HttpGet("email")]
    public async Task<ActionResult<bool>> CheckUserByEmail(string email, CancellationToken cancellationToken)
    {
        var result = await authentication.CheckEmailExistsAsync(email, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("currentUser")]
    [Authorize]
    public async Task<ActionResult<UserDto>>GetCurrentUser( CancellationToken cancellationToken)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException();
        var result = await authentication.GetCurrentUserAsync(email, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("currentAddress")]
    [Authorize]
    public async Task<ActionResult<AddressDto>> GetCurrentAddress(CancellationToken cancellationToken)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException();
        var result = await authentication.GetCurrentUserAddressAsync(email, cancellationToken);
        return ToActionResult(result);
    }


}
