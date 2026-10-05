using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services.Contracts
{
    public interface IAuthenticationService
    {
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default);
        Task<Result<UserDto>> RegistrationAsync(RegistrationDto registration, CancellationToken ct = default);
        Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken ct);
        Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken ct);
        Task<Result<AddressDto>> GetCurrentUserAddressAsync(string email, CancellationToken ct = default);



    }
}
