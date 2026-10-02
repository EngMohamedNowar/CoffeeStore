using System.Text;
using ECommerce.Application.Services.Classes.Authentications;
using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Entities.Identity;
using ECommerce.Infrastructure.Identity.Services;
using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Persistence.DataSeeding;
using ECommerce.Infrastructure.Persistence.Identity.Data;
using ECommerce.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace ECommerce.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddDbContext<StoreIdentityDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
        });

        services.AddKeyedScoped<IDataSeeder, CatalogDataSeeder>("Catalog");
        services.AddKeyedScoped<IDataSeeder, IdentityDataSeeder>("Identity");

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(
                configuration.GetConnectionString("RedisConnection")
                ?? throw new InvalidOperationException("ConnectionStrings:RedisConnection is not configured.")));

        services.AddIdentityCore<ApplicationUser>(options => { options.User.RequireUniqueEmail = true; })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<StoreIdentityDbContext>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<ICacheRepository, CacheRepository>();
        services.AddScoped<IIdentityService, IdentityService>();
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                          ?? new JwtOptions();

        if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey is not configured. Set it through user-secrets, environment variables, or a secret store before starting the API.");
        }

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}
