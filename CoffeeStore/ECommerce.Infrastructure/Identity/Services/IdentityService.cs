using ECommerce.Application.Common;
using ECommerce.Application.Common.Models;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Entities.Identity;
using ECommerce.Infrastructure.Persistence.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Identity.Services;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    StoreIdentityDbContext context) : IIdentityService
{
    public async Task<Result<AuthUserSnapshot>> CreateUserAsync(
        string email,
        string password,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName ?? string.Empty,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e =>
                    e.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return Result<AuthUserSnapshot>.Fail(IdentityErrors.EmailAlreadyExists);
            }

            return Result<AuthUserSnapshot>.Fail(IdentityErrors.CreateFailed(Describe(result)));
        }

        var roleResult = await userManager.AddToRoleAsync(user, Roles.User);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.CreateFailed(Describe(roleResult)));
        }

        return Result<AuthUserSnapshot>.Ok(ToSnapshot(user));
    }

    public async Task<Result<AuthUserSnapshot>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.InvalidCredentials);

        var isValid = await userManager.CheckPasswordAsync(user, password);
        if (!isValid)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.InvalidCredentials);

        if (!user.EmailConfirmed)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.EmailNotConfirmed);

        return Result<AuthUserSnapshot>.Ok(ToSnapshot(user));
    }

    public async Task<Result<AuthUserSnapshot>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.UserNotFound);

        return Result<AuthUserSnapshot>.Ok(ToSnapshot(user));
    }

    public async Task<Result<AuthUserSnapshot>> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.UserNotFound);

        return Result<AuthUserSnapshot>.Ok(ToSnapshot(user));
    }

    public async Task<Result<AuthUserSnapshot>> UpdateProfileAsync(
        Guid userId,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.UserNotFound);

        user.DisplayName = string.IsNullOrWhiteSpace(displayName) ? string.Empty : displayName.Trim();
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return Result<AuthUserSnapshot>.Fail(IdentityErrors.Validation(Describe(result)));

        return Result<AuthUserSnapshot>.Ok(ToSnapshot(user));
    }

    public async Task<Result> ConfirmEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Fail(IdentityErrors.UserNotFound);

        if (user.EmailConfirmed)
            return Result.Fail(IdentityErrors.EmailAlreadyConfirmed);

        user.EmailConfirmed = true;
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Ok()
            : Result.Fail(IdentityErrors.CreateFailed(Describe(result)));
    }

    public async Task<bool> IsEmailConfirmedAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user?.EmailConfirmed ?? false;
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return [];

        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<Result<AddressDto>> GetCurrentUserAddressAsync(
        string email,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AddressDto>.Fail(IdentityErrors.UserNotFound);

        var address = await OrderedAddresses(user.Id).FirstOrDefaultAsync(ct);
        if (address is null)
            return Result<AddressDto>.Fail(AddressErrors.AddressNotFound);

        return Result<AddressDto>.Ok(ToDto(address));
    }

    public async Task<Result<IReadOnlyList<AddressDto>>> GetAddressesAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<IReadOnlyList<AddressDto>>.Fail(IdentityErrors.UserNotFound);

        var addresses = await OrderedAddresses(user.Id).ToListAsync(cancellationToken);

        return Result<IReadOnlyList<AddressDto>>.Ok(addresses.Select(ToDto).ToList());
    }

    public async Task<Result<AddressDto>> AddAddressAsync(
        string email,
        AddressDto addressDto,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AddressDto>.Fail(IdentityErrors.UserNotFound);

        var invalid = Validate(addressDto);
        if (invalid is not null)
            return Result<AddressDto>.Fail(invalid);

        var isFirstAddress = !await context.Addresses.AnyAsync(a => a.UserId == user.Id, cancellationToken);
        var makeDefault = addressDto.IsDefault || isFirstAddress;

        if (makeDefault)
            await ClearDefaultAsync(user.Id, cancellationToken);

        var address = new Address
        {
            Label = addressDto.Label.Trim(),
            FirstName = addressDto.FirstName.Trim(),
            LastName = addressDto.LastName.Trim(),
            Country = addressDto.Country.Trim(),
            City = addressDto.City.Trim(),
            Street = addressDto.Street.Trim(),
            Building = addressDto.Building?.Trim(),
            Notes = addressDto.Notes?.Trim(),
            IsDefault = makeDefault,
            UserId = user.Id
        };

        context.Addresses.Add(address);
        await context.SaveChangesAsync(cancellationToken);

        return Result<AddressDto>.Ok(ToDto(address));
    }

    public async Task<Result<AddressDto>> UpdateAddressAsync(
        string email,
        Guid addressId,
        AddressDto addressDto,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AddressDto>.Fail(IdentityErrors.UserNotFound);

        var invalid = Validate(addressDto);
        if (invalid is not null)
            return Result<AddressDto>.Fail(invalid);

        var address = await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == user.Id, cancellationToken);

        if (address is null)
            return Result<AddressDto>.Fail(AddressErrors.AddressNotFound);

        if (addressDto.IsDefault && !address.IsDefault)
            await ClearDefaultAsync(user.Id, cancellationToken);

        address.Label = addressDto.Label.Trim();
        address.FirstName = addressDto.FirstName.Trim();
        address.LastName = addressDto.LastName.Trim();
        address.Country = addressDto.Country.Trim();
        address.City = addressDto.City.Trim();
        address.Street = addressDto.Street.Trim();
        address.Building = addressDto.Building?.Trim();
        address.Notes = addressDto.Notes?.Trim();
        address.IsDefault = addressDto.IsDefault;

        await context.SaveChangesAsync(cancellationToken);

        return Result<AddressDto>.Ok(ToDto(address));
    }

    public async Task<Result> DeleteAddressAsync(
        string email,
        Guid addressId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Fail(IdentityErrors.UserNotFound);

        var address = await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == user.Id, cancellationToken);

        if (address is null)
            return Result.Fail(AddressErrors.AddressNotFound);

        context.Addresses.Remove(address);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    private static AuthUserSnapshot ToSnapshot(ApplicationUser user)
        => new(Guid.Parse(user.Id), user.Email ?? string.Empty, user.DisplayName, user.UserName ?? string.Empty);

    private static string Describe(IdentityResult result)
        => string.Join(" ", result.Errors.Select(e => e.Description));

    private IQueryable<Address> OrderedAddresses(string userId)
        => context.Addresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.Label);

    private async Task ClearDefaultAsync(string userId, CancellationToken cancellationToken)
    {
        var defaults = await context.Addresses
            .Where(a => a.UserId == userId && a.IsDefault)
            .ToListAsync(cancellationToken);

        foreach (var existing in defaults)
        {
            existing.IsDefault = false;
        }
    }

    private static Error? Validate(AddressDto address)
        => string.IsNullOrWhiteSpace(address.Street) || string.IsNullOrWhiteSpace(address.City)
            ? AddressErrors.AddressInvalid
            : null;

    private static AddressDto ToDto(Address address) => new()
    {
        Id = address.Id,
        Label = address.Label,
        FirstName = address.FirstName,
        LastName = address.LastName,
        Country = address.Country,
        City = address.City,
        Street = address.Street,
        Building = address.Building,
        Notes = address.Notes,
        IsDefault = address.IsDefault
    };
}
