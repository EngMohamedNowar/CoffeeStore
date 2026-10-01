using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Identity;

namespace ECommerce.Application.Services.Contracts;

public interface IIdentitityServices
{
    Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default);

    Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default);

    Task<Result<IdentityUserResult>> CreateUserAsync(RegistrationDto registrationDto, CancellationToken ct = default);
    Task<Result<IReadOnlyList<string>>> GetUserRoleAsync(string email, CancellationToken ct = default);

}
