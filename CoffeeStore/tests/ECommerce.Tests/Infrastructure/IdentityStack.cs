using ECommerce.Application.Common;
using ECommerce.Domain.Entities.Identity;
using ECommerce.Infrastructure.Identity.Services;
using ECommerce.Infrastructure.Persistence.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using DomainRoles = ECommerce.Domain.Entities.Identity.Roles;

namespace ECommerce.Tests.Infrastructure;

internal sealed class IdentityStack : IDisposable
{
    public const string DefaultEmail = "shopper@store.com";
    public const string DefaultPassword = "Passw0rd!";

    public StoreIdentityDbContext Context { get; }
    public UserManager<ApplicationUser> UserManager { get; }
    public IdentityService Identity { get; }

    public IdentityStack()
    {
        var options = new DbContextOptionsBuilder<StoreIdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new StoreIdentityDbContext(options);
        Context.Roles.Add(new IdentityRole
        {
            Name = DomainRoles.User,
            NormalizedName = DomainRoles.User.ToUpperInvariant(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        });
        Context.SaveChanges();

        UserManager = BuildUserManager(Context);
        Identity = new IdentityService(UserManager, Context);
    }

    public async Task<string> CreateUserAsync(
        string email = DefaultEmail,
        bool confirmed = true)
    {
        var created = await Identity.CreateUserAsync(email, DefaultPassword, "Test Shopper");
        Assert.True(created.IsSuccess, Describe(created.Errors));

        if (confirmed)
        {
            var confirmedResult = await Identity.ConfirmEmailAsync(email);
            Assert.True(confirmedResult.IsSuccess, Describe(confirmedResult.Errors));
        }

        return email;
    }

    private static UserManager<ApplicationUser> BuildUserManager(StoreIdentityDbContext context)
    {
        var store = new UserStore<ApplicationUser, IdentityRole, StoreIdentityDbContext>(context);

        // Mirrors InfrastructureServicesRegistration: UserValidator is what rejects a
        // duplicate email, so both the validator and RequireUniqueEmail must be wired here.
        var options = new IdentityOptions { User = { RequireUniqueEmail = true } };

        return new UserManager<ApplicationUser>(
            store,
            Options.Create(options),
            new PasswordHasher<ApplicationUser>(),
            new IUserValidator<ApplicationUser>[] { new UserValidator<ApplicationUser>() },
            new IPasswordValidator<ApplicationUser>[] { new PasswordValidator<ApplicationUser>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new ServiceCollection().BuildServiceProvider(),
            NullLogger<UserManager<ApplicationUser>>.Instance);
    }

    private static string Describe(IReadOnlyList<Error> errors)
        => string.Join(", ", errors.Select(e => $"{e.code}: {e.description}"));

    public void Dispose()
    {
        UserManager.Dispose();
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}
