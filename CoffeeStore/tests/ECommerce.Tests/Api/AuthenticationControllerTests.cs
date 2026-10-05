using ECommerce.Api.Controllers;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Classes.Authentications;
using ECommerce.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ECommerce.Tests.Api;

public class AuthenticationControllerTests
{
    private const string SigningKey = "unit-test-signing-key-that-is-long-enough-0123456789";

    private static (AuthenticationController controller, IdentityStack stack) Build(string? email = null)
    {
        var stack = new IdentityStack();
        var tokens = new TokenServices(Options.Create(new JwtOptions
        {
            Issuer = "ECommerce.Tests",
            Audience = "ECommerce.Tests.Client",
            SigningKey = SigningKey,
            ExpiryMinutes = 15
        }));

        var controller = new AuthenticationController(new AuthenticationsServices(stack.Identity, tokens))
        {
            ControllerContext = email is null
                ? ControllerContextFactory.WithoutEmail()
                : ControllerContextFactory.WithEmail(email)
        };

        return (controller, stack);
    }

    private static RegistrationDto NewRegistration(string email = "joiner@store.com") => new()
    {
        Email = email,
        Password = IdentityStack.DefaultPassword,
        UserName = email,
        DisplayName = "Joiner"
    };

    private static T Value<T>(ActionResult<T> action)
        => Assert.IsType<T>(Assert.IsType<OkObjectResult>(action.Result).Value);

    private static int? Status(ActionResult action)
        => Assert.IsType<ObjectResult>(action).StatusCode;

    [Fact]
    public async Task Register_ThenLogin_ReturnsAGuidIdAndAToken()
    {
        var (controller, stack) = Build();
        using var _ = stack;

        var registration = Value(await controller.Register(NewRegistration()));
        Assert.NotEqual(Guid.Empty, registration.Id);

        await stack.Identity.ConfirmEmailAsync(registration.Email);

        var login = Value(await controller.Login(new LoginDto
        {
            Email = registration.Email,
            Password = IdentityStack.DefaultPassword
        }));

        Assert.Equal(registration.Id, login.Id);
        Assert.False(string.IsNullOrWhiteSpace(login.Token));
    }

    [Fact]
    public async Task Login_Returns401_ForAWrongPassword()
    {
        var (controller, stack) = Build();
        using var _ = stack;

        await stack.CreateUserAsync();
        await stack.Identity.ConfirmEmailAsync(IdentityStack.DefaultEmail);

        var action = await controller.Login(new LoginDto
        {
            Email = IdentityStack.DefaultEmail,
            Password = "WrongPass1!"
        });

        Assert.Equal(401, Status(action.Result!));
    }

    [Fact]
    public async Task CheckUserByEmail_ReportsWhetherTheAddressIsTaken()
    {
        var (controller, stack) = Build();
        using var _ = stack;

        await stack.CreateUserAsync();

        Assert.True(Value(await controller.CheckUserByEmail(IdentityStack.DefaultEmail, default)));

        var missing = await controller.CheckUserByEmail("missing@store.com", default);
        Assert.Equal(404, Status(missing.Result!));
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsTheGuidIdOfTheSignedInUser()
    {
        var (controller, stack) = Build(IdentityStack.DefaultEmail);
        using var _ = stack;

        await stack.CreateUserAsync();

        var action = await controller.GetCurrentUser(default);

        var user = Value(action);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(IdentityStack.DefaultEmail, user.Email);
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsNotFound_ForAnUnknownClaim()
    {
        var (controller, stack) = Build("ghost@store.com");
        using var _ = stack;

        var action = await controller.GetCurrentUser(default);

        Assert.Equal(404, Status(action.Result!));
    }

    [Fact]
    public async Task ProtectedActions_RequireAnEmailClaim()
    {
        var (controller, stack) = Build();
        using var _ = stack;

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.GetCurrentUser(default));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.GetCurrentAddress(default));
    }

    [Fact]
    public async Task GetCurrentAddress_ReturnsNotFound_WhenTheUserHasNoAddress()
    {
        var (controller, stack) = Build(IdentityStack.DefaultEmail);
        using var _ = stack;

        await stack.CreateUserAsync();

        var action = await controller.GetCurrentAddress(default);

        Assert.Equal(404, Status(action.Result!));
    }
}
