using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Classes.Authentications;
using ECommerce.Tests.Infrastructure;
using Microsoft.Extensions.Options;

namespace ECommerce.Tests.Application;

public class AuthenticationServicesTests
{
    private const string SigningKey = "unit-test-signing-key-that-is-long-enough-0123456789";

    private static (IdentityStack stack, AuthenticationsServices service) BuildService()
    {
        var stack = new IdentityStack();
        var tokens = new TokenServices(Options.Create(new JwtOptions
        {
            Issuer = "ECommerce.Tests",
            Audience = "ECommerce.Tests.Client",
            SigningKey = SigningKey,
            ExpiryMinutes = 15
        }));

        return (stack, new AuthenticationsServices(stack.Identity, tokens));
    }

    private static AddressDto NewAddress(string street = "Main St") => new()
    {
        Label = "Home",
        FirstName = "Sara",
        LastName = "Nasser",
        Country = "Egypt",
        City = "Cairo",
        Street = street
    };

    [Fact]
    public async Task RegistrationAsync_ReturnsAUserWithAGuidIdAndAToken()
    {
        var (stack, service) = BuildService();
        using var _ = stack;

        var result = await service.RegistrationAsync(new RegistrationDto
        {
            Email = "joiner@store.com",
            Password = IdentityStack.DefaultPassword,
            DisplayName = "Joiner"
        });

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.NotEqual(Guid.Empty, result.Value!.Id);
        Assert.Equal("joiner@store.com", result.Value.Email);
        Assert.Equal("Joiner", result.Value.DisplayName);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.Token));
        Assert.NotNull(await stack.UserManager.FindByEmailAsync("joiner@store.com"));
    }

    [Fact]
    public async Task RegistrationAsync_RejectsADuplicateEmail()
    {
        var (stack, service) = BuildService();
        using var _ = stack;

        var registration = new RegistrationDto
        {
            Email = "twice@store.com",
            Password = IdentityStack.DefaultPassword,
            DisplayName = "Twice"
        };

        var first = await service.RegistrationAsync(registration);
        var second = await service.RegistrationAsync(registration);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsFailure);
        Assert.Equal(ErrorType.Conflict, Assert.Single(second.Errors).ErrorType);
    }

    [Fact]
    public async Task LoginAsync_IsBlockedUntilTheEmailIsConfirmed()
    {
        var (stack, service) = BuildService();
        using var _ = stack;
        await stack.CreateUserAsync(confirmed: false);

        var blocked = await service.LoginAsync(new LoginDto
        {
            Email = IdentityStack.DefaultEmail,
            Password = IdentityStack.DefaultPassword
        });

        Assert.True(blocked.IsFailure);
        Assert.Equal(ErrorType.Forbidden, Assert.Single(blocked.Errors).ErrorType);

        await stack.Identity.ConfirmEmailAsync(IdentityStack.DefaultEmail);

        var allowed = await service.LoginAsync(new LoginDto
        {
            Email = IdentityStack.DefaultEmail,
            Password = IdentityStack.DefaultPassword
        });

        Assert.True(allowed.IsSuccess, string.Join(", ", allowed.Errors.Select(e => e.code)));
        Assert.False(string.IsNullOrWhiteSpace(allowed.Value!.Token));
    }

    [Fact]
    public async Task LoginAsync_RejectsAWrongPassword()
    {
        var (stack, service) = BuildService();
        using var _ = stack;
        await stack.CreateUserAsync();

        var result = await service.LoginAsync(new LoginDto
        {
            Email = IdentityStack.DefaultEmail,
            Password = "NotThePassword1!"
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Unauthorized, Assert.Single(result.Errors).ErrorType);
    }

    [Fact]
    public async Task CheckEmailExistsAsync_TellsNewFromKnownAddresses()
    {
        var (stack, service) = BuildService();
        using var _ = stack;
        await stack.CreateUserAsync();

        var known = await service.CheckEmailExistsAsync(IdentityStack.DefaultEmail, default);
        var unknown = await service.CheckEmailExistsAsync("missing@store.com", default);

        Assert.True(known.IsSuccess);
        Assert.True(known.Value);
        Assert.False(unknown.IsSuccess);
        Assert.Equal(ErrorType.NotFound, Assert.Single(unknown.Errors).ErrorType);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsTheStoredIdAndAFreshToken()
    {
        var (stack, service) = BuildService();
        using var _ = stack;
        await stack.CreateUserAsync();

        var result = await service.GetCurrentUserAsync(IdentityStack.DefaultEmail, default);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        var stored = await stack.UserManager.FindByEmailAsync(IdentityStack.DefaultEmail);
        Assert.Equal(Guid.Parse(stored!.Id), result.Value!.Id);
        Assert.Equal(IdentityStack.DefaultEmail, result.Value.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.Token));
    }

    [Fact]
    public async Task GetCurrentUserAsync_FailsForAnUnknownUser()
    {
        var (stack, service) = BuildService();
        using var _ = stack;

        var result = await service.GetCurrentUserAsync("missing@store.com", default);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, Assert.Single(result.Errors).ErrorType);
    }

    [Fact]
    public async Task GetCurrentUserAddressAsync_ReportsMissingThenReturnsTheSavedAddress()
    {
        var (stack, service) = BuildService();
        using var _ = stack;
        await stack.CreateUserAsync();

        var beforeSaving = await service.GetCurrentUserAddressAsync(IdentityStack.DefaultEmail, default);

        Assert.True(beforeSaving.IsFailure);
        Assert.Equal("Address.NotFound", Assert.Single(beforeSaving.Errors).code);

        await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress("Corniche St"));

        var afterSaving = await service.GetCurrentUserAddressAsync(IdentityStack.DefaultEmail, default);

        Assert.True(afterSaving.IsSuccess, string.Join(", ", afterSaving.Errors.Select(e => e.code)));
        Assert.Equal("Corniche St", afterSaving.Value!.Street);
        Assert.Equal("Sara Nasser", afterSaving.Value.FullName);
    }
}
