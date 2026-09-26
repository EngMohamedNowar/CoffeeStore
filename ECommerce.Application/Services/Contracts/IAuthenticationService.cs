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
    }
}
