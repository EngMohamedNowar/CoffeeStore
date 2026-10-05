using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Application.Services.Classes.Authentications;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Tests.Application;

public class TokenServicesTests
{
    private const string SigningKey = "unit-test-signing-key-that-is-long-enough-0123456789";

    private static TokenServices BuildService(string? signingKey = SigningKey, int expiryMinutes = 15)
        => new(Options.Create(new JwtOptions
        {
            Issuer = "ECommerce.Tests",
            Audience = "ECommerce.Tests.Client",
            SigningKey = signingKey ?? string.Empty,
            ExpiryMinutes = expiryMinutes
        }));

    [Fact]
    public async Task CreateTokenAsync_FailsWhenTheSigningKeyIsMissing()
    {
        var result = await BuildService(signingKey: string.Empty)
            .CreateTokenAsync(Guid.NewGuid(), "a@store.com", "A", ["User"]);

        Assert.True(result.IsFailure);
        Assert.Equal("Token.NotConfigured", Assert.Single(result.Errors).code);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task CreateTokenAsync_FailsWhenTheSigningKeyIsTooShortForHmacSha256()
    {
        var result = await BuildService(signingKey: "too-short")
            .CreateTokenAsync(Guid.NewGuid(), "a@store.com", "A", ["User"]);

        Assert.True(result.IsFailure);
        Assert.Equal("Token.NotConfigured", Assert.Single(result.Errors).code);
    }

    [Fact]
    public async Task CreateTokenAsync_EmptyRolesStillProducesAToken()
    {
        var result = await BuildService()
            .CreateTokenAsync(Guid.NewGuid(), "a@store.com", "A", []);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.False(string.IsNullOrWhiteSpace(result.Value));
    }

    [Fact]
    public async Task CreateTokenAsync_CarriesTheUserClaims()
    {
        var userId = Guid.NewGuid();
        var result = await BuildService()
            .CreateTokenAsync(userId, "sara@store.com", "Sara Nasser", ["User", "Admin"]);

        Assert.True(result.IsSuccess);

        var principal = Validate(result.Value!);

        Assert.Equal(userId.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal("sara@store.com", principal.FindFirst(ClaimTypes.Email)?.Value);
        Assert.Equal("Sara Nasser", principal.FindFirst(ClaimTypes.Name)?.Value);
        Assert.False(string.IsNullOrWhiteSpace(principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value));
        Assert.True(principal.IsInRole("User"));
        Assert.True(principal.IsInRole("Admin"));
    }

    [Fact]
    public async Task CreateTokenAsync_SetsIssuerAudienceAndExpiry()
    {
        var before = DateTime.UtcNow;

        var result = await BuildService(expiryMinutes: 10)
            .CreateTokenAsync(Guid.NewGuid(), "a@store.com", "A", []);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Value);

        Assert.Equal("ECommerce.Tests", token.Issuer);
        Assert.Equal("ECommerce.Tests.Client", Assert.Single(token.Audiences));
        Assert.True(token.ValidTo >= before.AddMinutes(9), $"ValidTo {token.ValidTo:o} is too early");
        Assert.True(token.ValidTo <= DateTime.UtcNow.AddMinutes(11), $"ValidTo {token.ValidTo:o} is too late");
    }

    [Fact]
    public async Task CreateTokenAsync_IssuesADistinctTokenOnEveryCall()
    {
        var service = BuildService();
        var userId = Guid.NewGuid();

        var first = await service.CreateTokenAsync(userId, "a@store.com", "A", ["User"]);
        var second = await service.CreateTokenAsync(userId, "a@store.com", "A", ["User"]);

        Assert.NotEqual(first.Value, second.Value);
    }

    private static ClaimsPrincipal Validate(string token)
    {
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = true };

        return handler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey)),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        }, out _);
    }
}
