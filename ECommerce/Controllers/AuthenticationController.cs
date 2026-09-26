using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    public class AuthenticationController(IAuthenticationService authentication)  : ApiControllerBase
    {
        public async Task<ActionResult<UserDto>> Login(LoginDto login,CancellationToken ct)
        {
            var result = await authentication.LoginAsync(login, ct);
            return ToActionResult(result);

        }
    }
}
