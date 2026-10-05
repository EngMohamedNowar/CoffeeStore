using ECommerce.Application.Common;
using ECommerce.Domain.Entities.Identity;

namespace ECommerce.Tests.Infrastructure;

public class IdentityServiceTests
{
    [Fact]
    public async Task CreateUserAsync_PersistsTheUserAndAssignsTheUserRole()
    {
        using var stack = new IdentityStack();

        var result = await stack.Identity.CreateUserAsync("new@store.com", IdentityStack.DefaultPassword, "Nada");

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal("new@store.com", result.Value!.Email);
        Assert.Equal("Nada", result.Value.DisplayName);

        var stored = await stack.UserManager.FindByEmailAsync("new@store.com");
        Assert.NotNull(stored);
        Assert.False(stored!.EmailConfirmed);

        var roles = await stack.Identity.GetRolesAsync(Guid.Parse(stored.Id));
        Assert.Equal(new[] { Roles.User }, roles);
    }

    [Fact]
    public async Task CreateUserAsync_RejectsADuplicateEmail()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var result = await stack.Identity.CreateUserAsync(
            IdentityStack.DefaultEmail,
            IdentityStack.DefaultPassword,
            "Other");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, Assert.Single(result.Errors).ErrorType);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsASnapshotWithAGuidId()
    {
        using var stack = new IdentityStack();
        var email = await stack.CreateUserAsync();

        var result = await stack.Identity.GetUserByEmailAsync(email);

        Assert.True(result.IsSuccess);
        Assert.Equal(Guid.Parse((await stack.UserManager.FindByEmailAsync(email))!.Id), result.Value!.Id);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal("Test Shopper", result.Value.DisplayName);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.UserName));
    }

    [Fact]
    public async Task GetUserByEmailAsync_FailsForAnUnknownAddress()
    {
        using var stack = new IdentityStack();

        var result = await stack.Identity.GetUserByEmailAsync("ghost@store.com");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, Assert.Single(result.Errors).ErrorType);
    }

    [Fact]
    public async Task GetUserByIdAsync_RoundTripsTheGuid()
    {
        using var stack = new IdentityStack();
        var email = await stack.CreateUserAsync();
        var stored = await stack.UserManager.FindByEmailAsync(email);
        var userId = Guid.Parse(stored!.Id);

        var found = await stack.Identity.GetUserByIdAsync(userId);
        var missing = await stack.Identity.GetUserByIdAsync(Guid.NewGuid());

        Assert.True(found.IsSuccess);
        Assert.Equal(userId, found.Value!.Id);
        Assert.True(missing.IsFailure);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_RejectsAWrongPassword()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var result = await stack.Identity.ValidateCredentialsAsync(IdentityStack.DefaultEmail, "WrongPass1!");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Unauthorized, Assert.Single(result.Errors).ErrorType);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_BlocksAnUnconfirmedEmail()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync(confirmed: false);

        var result = await stack.Identity.ValidateCredentialsAsync(IdentityStack.DefaultEmail, IdentityStack.DefaultPassword);

        Assert.True(result.IsFailure);
        Assert.Equal("Identity.EmailNotConfirmed", Assert.Single(result.Errors).code);
        Assert.Equal(ErrorType.Forbidden, result.Errors[0].ErrorType);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_AcceptsAConfirmedUser()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var result = await stack.Identity.ValidateCredentialsAsync(IdentityStack.DefaultEmail, IdentityStack.DefaultPassword);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(IdentityStack.DefaultEmail, result.Value!.Email);
    }

    [Fact]
    public async Task ConfirmEmailAsync_IsIdempotentOnlyOnce()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync(confirmed: false);
        Assert.False(await stack.Identity.IsEmailConfirmedAsync(IdentityStack.DefaultEmail));

        var first = await stack.Identity.ConfirmEmailAsync(IdentityStack.DefaultEmail);
        var second = await stack.Identity.ConfirmEmailAsync(IdentityStack.DefaultEmail);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsFailure);
        Assert.Equal(ErrorType.Conflict, Assert.Single(second.Errors).ErrorType);
        Assert.True(await stack.Identity.IsEmailConfirmedAsync(IdentityStack.DefaultEmail));
    }

    [Fact]
    public async Task UpdateProfileAsync_ChangesTheDisplayName()
    {
        using var stack = new IdentityStack();
        var email = await stack.CreateUserAsync();
        var userId = Guid.Parse((await stack.UserManager.FindByEmailAsync(email))!.Id);

        var updated = await stack.Identity.UpdateProfileAsync(userId, "  Renamed  ");
        var blank = await stack.Identity.UpdateProfileAsync(Guid.NewGuid(), "Nobody");

        Assert.True(updated.IsSuccess);
        Assert.Equal("Renamed", updated.Value!.DisplayName);
        Assert.True(blank.IsFailure);
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsNothingForAnUnknownUser()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();
        var userId = Guid.Parse((await stack.UserManager.FindByEmailAsync(IdentityStack.DefaultEmail))!.Id);

        var known = await stack.Identity.GetRolesAsync(userId);
        var unknown = await stack.Identity.GetRolesAsync(Guid.NewGuid());

        Assert.Equal(new[] { Roles.User }, known);
        Assert.Empty(unknown);
    }
}
