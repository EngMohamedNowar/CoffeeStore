using ECommerce.Application.Common;
using ECommerce.Application.Common.Models;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Contracts;

namespace ECommerce.Application.Services.Classes.Authentications;

public class AuthenticationsServices(
    IIdentityService identityService,
    ITokenServices tokenServices) : IAuthenticationService
{
    public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
    {
        var userResult = await identityService.ValidateCredentialsAsync(
            loginDto.Email,
            loginDto.Password,
            ct);

        if (!userResult.IsSuccess)
        {
            return Result<UserDto>.Fail(userResult.Errors);
        }

        return await CreateSignedInResultAsync(userResult.Value!, ct);
    }

    public async Task<Result<UserDto>> RegistrationAsync(RegistrationDto registration, CancellationToken ct = default)
    {
        var userResult = await identityService.CreateUserAsync(
            registration.Email,
            registration.Password,
            registration.DisplayName,
            ct);

        if (!userResult.IsSuccess)
        {
            return Result<UserDto>.Fail(userResult.Errors);
        }

        return await CreateSignedInResultAsync(userResult.Value!, ct);
    }

    private async Task<Result<UserDto>> CreateSignedInResultAsync(
        AuthUserSnapshot user,
        CancellationToken ct)
    {
        var roles = await identityService.GetRolesAsync(user.Id, ct);

        var token = await tokenServices.CreateTokenAsync(
            user.Id.ToString(),
            user.Email,
            user.DisplayName ?? user.Email,
            roles,
            ct);

        if (!token.IsSuccess)
        {
            return Result<UserDto>.Fail(token.Errors);
        }

        return Result<UserDto>.Ok(new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName ?? string.Empty,
            Token = token.Value!
        });
    }
}
