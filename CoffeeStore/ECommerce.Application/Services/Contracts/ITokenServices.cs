using ECommerce.Application.Common;

namespace ECommerce.Application.Services.Contracts;

public interface ITokenServices
{
    Task<Result<string>> CreateTokenAsync(Guid userId, string email, string userName, IReadOnlyList<string> roles, CancellationToken ct = default);
}
