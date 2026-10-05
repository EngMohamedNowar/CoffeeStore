using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Tests.Api;

internal static class ControllerContextFactory
{
    public static ControllerContext WithEmail(string email)
        => Build(new ClaimsIdentity([new Claim(ClaimTypes.Email, email)], "TestAuth"));

    public static ControllerContext WithoutEmail()
        => Build(new ClaimsIdentity());

    private static ControllerContext Build(ClaimsIdentity identity)
        => new()
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
}
