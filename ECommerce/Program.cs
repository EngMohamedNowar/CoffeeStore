using ECommerce.Api.Extensions;
using ECommerce.Application;
using ECommerce.Domain.Entities.Identity;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using IdentityRole = Microsoft.AspNetCore.Identity.IdentityRole;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplictaionServices();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<StoreDbContext>()
.AddDefaultTokenProviders();


var app = builder.Build();

await app.SeedAndMigrationAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
