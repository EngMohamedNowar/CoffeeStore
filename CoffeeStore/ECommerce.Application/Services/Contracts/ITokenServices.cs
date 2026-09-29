using ECommerce.Application.Common;

namespace ECommerce.Application.Services.Contracts;

public interface ITokenServices
{
    Task<Result<string>> CreateTokenAsync(string userId, string email, string userName, IReadOnlyList<string> roles, CancellationToken ct = default);
}
