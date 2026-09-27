using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Contracts;

namespace ECommerce.Application.Services.Classes.Authentications;

public class AuthenticationsServices(
    IIdentitityServices identitityServices,
    ITokenServices tokenServices) : IAuthenticationService
{
    public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
    {
        var userResult = await identitityServices.FindUserByEmailAsync(loginDto.Email, ct);
        if (!userResult.IsSuccess)
        {
            return Result<UserDto>.Fail(userResult.Errors);
        }

        var passwordResult = await identitityServices.CheckPasswordAsync(loginDto.Email, loginDto.Password, ct);
        if (!passwordResult.IsSuccess)
        {
            return Result<UserDto>.Fail(passwordResult.Errors);
        }

        var user = userResult.Value!;

        var token = await tokenServices.CreateTokenAsync(user.Id, user.Email, user.UserName, ct);
        if (!token.IsSuccess)
        {
            return Result<UserDto>.Fail(token.Errors);
        }

        return Result<UserDto>.Ok(new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = token.Value!
        });
    }

    public async Task<Result<UserDto>> RegistrationAsync(RegistrationDto registration, CancellationToken ct = default)
    {
        var userRegistration = await identitityServices.CreateUserAsync(registration, ct);
        if (!userRegistration.IsSuccess)
        {
            return Result<UserDto>.Fail(userRegistration.Errors);
        }

        var user = userRegistration.Value!;

        var token = await tokenServices.CreateTokenAsync(user.Id, user.Email, user.UserName, ct);
        if (!token.IsSuccess)
        {
            return Result<UserDto>.Fail(token.Errors);
        }

        return Result<UserDto>.Ok(new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = token.Value!
        });
    }
}
