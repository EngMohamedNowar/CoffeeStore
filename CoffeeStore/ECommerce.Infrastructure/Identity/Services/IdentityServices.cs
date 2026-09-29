using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity.Services;

public class IdentityServices(UserManager<ApplicationUser> userManager) : IIdentitityServices
{
    public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result<bool>.Fail(Error.InvalidCredentials());
        }

        var result = await userManager.CheckPasswordAsync(user, password);

        return result
            ? Result<bool>.Ok(true)
            : Result<bool>.Fail(Error.InvalidCredentials());
    }

    public async Task<Result<IdentityUserResult>> CreateUserAsync(RegistrationDto registrationDto, CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            Email = registrationDto.Email,
            PhoneNumber = registrationDto.PhoneNumber,
            DisplayName = registrationDto.DisplayName,
            UserName = registrationDto.UserName
        };

        var createdResult = await userManager.CreateAsync(user, registrationDto.Password);
        if (!createdResult.Succeeded)
        {
            var errors = createdResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
            return Result<IdentityUserResult>.Fail(errors);
        }

        return Result<IdentityUserResult>.Ok(ToResult(user));
    }

    public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result<IdentityUserResult>.Fail(Error.InvalidCredentials());
        }

        return Result<IdentityUserResult>.Ok(ToResult(user));
    }

    private static IdentityUserResult ToResult(ApplicationUser user)
        => new(user.Id, user.DisplayName, user.Email ?? string.Empty, user.UserName ?? string.Empty);



    public async Task<Result<IReadOnlyList<string>>> GetUserRoleAsync(string email, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result<IReadOnlyList<string>>.Fail(Error.InvalidCredentials());
        }
        var roles = await userManager.GetRolesAsync(user);
        return Result<IReadOnlyList<string>>.Ok(roles.ToList());

    }
}
