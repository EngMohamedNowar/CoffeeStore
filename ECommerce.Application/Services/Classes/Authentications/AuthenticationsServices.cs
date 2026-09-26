using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services.Classes.Authentications
{
    public class AuthenticationsServices(IIdentitityServices identitityServices) : IAuthenticationService
    {
        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            //check email
            var userResult = await identitityServices.FindUserByEmailAsync(loginDto.Email, ct);
            if (!userResult.IsSuccess)
                return Result<UserDto>.Fail(userResult.Errors);

            //check password
            var passwordResult = await identitityServices.CheckPasswordAsync(loginDto.Email, loginDto.Password, ct);
            if (!passwordResult.IsSuccess)
                return Result<UserDto>.Fail(passwordResult.Errors);

            var user = userResult.Value;
            return  Result<UserDto>.Ok(new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = "Token"
            });

            }

        public async Task<Result<UserDto>> RegistrationAsync(
            RegistrationDto registration,
            CancellationToken ct = default)
        {
            var userRegistration =
                await identitityServices.CreateUserAsync(registration, ct);

            if (!userRegistration.IsSuccess)
            {
                return Result<UserDto>.Fail(userRegistration.Errors);
            }

            var user = userRegistration.Value;

            if (user is null)
            {
                return Result<UserDto>.Fail(userRegistration.Errors);
            }

            return Result<UserDto>.Ok(new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = "Token"
            });
        }

    }
}

