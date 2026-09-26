using ECommerce.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services.Contracts
{
    public interface IIdentitityServices
    {
        Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct);
        Task<Result<bool>> CheckPasswordAsync(string email,string password, CancellationToken ct);

    }
}
