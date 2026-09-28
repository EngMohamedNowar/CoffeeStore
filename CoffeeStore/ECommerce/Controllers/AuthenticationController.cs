using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Contracts;
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
}
