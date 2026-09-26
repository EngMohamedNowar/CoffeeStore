using ECommerce.Application.Common;
using ECommerce.Application.Services.Contracts;
using ECommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Identity.Services
{
    public class IdentityServices(UserManager<ApplicationUser> userManager) : IIdentitityServices
    {
        public async Task<Result<bool>> CheckPasswordAsync(string email,string password, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Result<bool>.Fail(Error.NotFound("User.NotFound", $"User With Email {email} Not Found"));
            }
            var result = await userManager.CheckPasswordAsync(user, password);
            return result ? Result<bool>.Ok(result) :
                Result<bool>.Fail(Error.Validation("Invalid Password","Failed Password"));

        }

        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Result<IdentityUserResult>.Fail(Error.NotFound("User.NotFound", $"User With Email {email} Not Found"));
              
            }
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }
    }
}