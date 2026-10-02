using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.Identity;
using ECommerce.Infrastructure.Persistence.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Persistence.DataSeeding;

public class IdentityDataSeeder(
    StoreIdentityDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    ILogger<IdentityDataSeeder> logger) : IDataSeeder
{
    private const string SeedAdminPath = "Seed:Admin";

    public async Task SeedDataAsync(CancellationToken ct = default)
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync(ct);
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync(ct);
        }

        foreach (var roleName in new[] { Roles.Admin, Roles.SuperAdmin, Roles.User })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        if (await userManager.Users.AnyAsync(ct))
        {
            return;
        }

        var adminConfig = ReadAdminConfig();
        if (adminConfig is null)
        {
            return;
        }

        var (admin, password) = adminConfig.Value;
        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("Could not seed the default admin account: {Errors}", errors);
            return;
        }

        await userManager.AddToRoleAsync(admin, Roles.Admin);
    }

    private (ApplicationUser Admin, string Password)? ReadAdminConfig()
    {
        var section = configuration.GetSection(SeedAdminPath);
        var userName = section["UserName"];
        var email = section["Email"];
        var password = section["Password"];

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "Skipped seeding the default admin account because '{Path}' is not fully configured. Set UserName, Email and Password through user-secrets or environment variables.",
                SeedAdminPath);
            return null;
        }

        var admin = new ApplicationUser
        {
            DisplayName = section["DisplayName"] ?? userName,
            UserName = userName,
            Email = email,
            PhoneNumber = section["PhoneNumber"]
        };

        return (admin, password);
    }
}
