using ECommerce.Application.Common;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity.Services;

public sealed class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<Result<AuthUserSnapshot>> CreateUserAsync(
        string email,
        string password,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName ?? string.Empty,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e =>
                    e.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return Result<AuthUserSnapshot>.Fail(IdentityErrors.EmailAlreadyExists);
            }

            return Result<AuthUserSnapshot>.Fail(IdentityErrors.CreateFailed(Describe(result)));
        }

        var roleResult = await userManager.AddToRoleAsync(user, Roles.User);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.CreateFailed(Describe(roleResult)));
        }

        return Result<AuthUserSnapshot>.Ok(ToSnapshot(user));
    }

    public async Task<Result<AuthUserSnapshot>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.InvalidCredentials);

        var isValid = await userManager.CheckPasswordAsync(user, password);
        if (!isValid)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.InvalidCredentials);

        if (!user.EmailConfirmed)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.EmailNotConfirmed);

        return Result<AuthUserSnapshot>.Ok(ToSnapshot(user));
    }

    public async Task<Result<AuthUserSnapshot>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.UserNotFound);

        return Result<AuthUserSnapshot>.Ok(ToSnapshot(user));
    }

    public async Task<Result<AuthUserSnapshot>> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.UserNotFound);

        return Result<AuthUserSnapshot>.Ok(ToSnapshot(user));
    }

    public async Task<Result<AuthUserSnapshot>> UpdateProfileAsync(
        Guid userId,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.UserNotFound);

        user.DisplayName = string.IsNullOrWhiteSpace(displayName) ? string.Empty : displayName.Trim();
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.Validation(Describe(result)));

        return Result<AuthUserSnapshot>.Ok(ToSnapshot(user));
    }

    public async Task<Result> ConfirmEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Fail(IdentityErrors.UserNotFound);

        if (user.EmailConfirmed)
            return Result.Fail(IdentityErrors.EmailAlreadyConfirmed);

        user.EmailConfirmed = true;
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Ok()
            : Result.Fail(IdentityErrors.CreateFailed(Describe(result)));
    }

    public async Task<bool> IsEmailConfirmedAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user?.EmailConfirmed ?? false;
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return [];

        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    private static AuthUserSnapshot ToSnapshot(ApplicationUser user)
        => new(Guid.Parse(user.Id), user.Email ?? string.Empty, user.DisplayName, user.UserName ?? string.Empty);

    private static string Describe(IdentityResult result)
        => string.Join(" ", result.Errors.Select(e => e.Description));
}
