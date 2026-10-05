using ECommerce.Application.Common;
using ECommerce.Application.Common.Models;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Contracts;

namespace ECommerce.Application.Services.Classes.Authentications;

public class AuthenticationsServices(
    IIdentityService identityService,
    ITokenServices tokenServices) : IAuthenticationService
{
    public async Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken ct)
    {
        var result = await identityService.GetUserByEmailAsync(email, ct);
        if (result.IsSuccess)
        {
            return Result<bool>.Ok(true);
        }
        return Result<bool>.Fail(result.Errors);
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken cancellationToken)
    {
        var result = await identityService.GetUserByEmailAsync(email, cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return Result<UserDto>.Fail(result.Errors);
        }

        var user = result.Value;
        var roles = await identityService.GetRolesAsync(user.Id, cancellationToken);
        var token = await tokenServices.CreateTokenAsync(
            user.Id,
            user.Email,
            user.UserName,
            roles,
            cancellationToken);

        if (!token.IsSuccess)
        {
            return Result<UserDto>.Fail(token.Errors);
        }

        return Result<UserDto>.Ok(new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName ?? string.Empty,
            Email = user.Email,
            Token = token.Value!
        });
    }

    public async Task<Result<AddressDto>> GetCurrentUserAddressAsync(string email, CancellationToken ct = default)
    {
        return await identityService.GetCurrentUserAddressAsync(email, ct);
    }

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
            user.Id,
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
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName ?? string.Empty,
            Token = token.Value!
        });
    }
}
